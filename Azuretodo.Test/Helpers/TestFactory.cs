using Azuretodo.Common.Models;
using Azuretodo.Functions.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Azuretodo.Test.Helpers
{
    public class TestFactory
    {
        public static DefaultHttpRequest CreateHttpRequest()
        {
            return new DefaultHttpRequest(new DefaultHttpContext());
        }

        public static DefaultHttpRequest CreateHttpRequest(Guid id)
        {
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Path = $"/{id}"
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(Todo dtoRequest)
        {
            string request = JsonConvert.SerializeObject(dtoRequest);

            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromString(request)
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(Guid id, Todo dtoRequest)
        {
            string request = JsonConvert.SerializeObject(dtoRequest);

            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromString(request),
                Path = $"/{id}"
            };
        }

        public static ILogger CreateLogger(LoggerTypes type = LoggerTypes.Null)
        {
            ILogger logger;

            logger = type == LoggerTypes.List
                ? new ListLogger()
                : NullLoggerFactory.Instance.CreateLogger("Null Logger");

            return logger;
        }

        public static Stream GenerateStreamFromString(string stringToConvert)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);

            writer.Write(stringToConvert);
            writer.Flush();
            stream.Position = 0;

            return stream;
        }
        public static TodoEntity GetTodoEntity()
        {
            return new TodoEntity
            {
                ETag = "*",
                PartitionKey = "TODO",
                RowKey = Guid.NewGuid().ToString(),
                CreatedTime = DateTime.UtcNow,
                IsCompleted = false,
                TaskDescription = "Task: enjoy life!"
            };
        }

        public static Todo GetTodoRequest()
        {
            return new Todo
            {
                CreatedTime = DateTime.UtcNow,
                IsCompleted = false,
                TaskDescription = "Try to conquer the World!"
            };
        }

        public static Todo GetTodoRequestWithNullDescriptionError()
        {
            return new Todo
            {
                CreatedTime = DateTime.UtcNow,
                IsCompleted = false,
                TaskDescription = string.Empty
            };
        }
    }
}
