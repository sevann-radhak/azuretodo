using Azuretodo.Common.Models;
using Azuretodo.Common.Responses;
using Azuretodo.Functions.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Azuretodo.Functions.Functions
{
    public static class TodoApi
    {
        [FunctionName(nameof(CreateTodo))]
        public static async Task<IActionResult> CreateTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todo")] HttpRequest req,
            [Table("todo", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
            ILogger log)
        {
            log.LogInformation("Recieved a new Todo.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Todo todo = JsonConvert.DeserializeObject<Todo>(requestBody);

            if (string.IsNullOrEmpty(todo?.TaskDescription))
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "The Description field is mandatory."
                });
            }

            TodoEntity todoEntity = new TodoEntity()
            {
                CreatedTime = DateTime.UtcNow,
                ETag = "*",
                IsCompleted = false,
                PartitionKey = "TODO",
                RowKey = Guid.NewGuid().ToString(),
                TaskDescription = todo.TaskDescription
            };

            TableOperation addOperation = TableOperation.Insert(todoEntity);
            await todoTable.ExecuteAsync(addOperation);

            string message = $"New Todo stored on table";
            log.LogInformation(message);

            return new ObjectResult(new Response
            {
                IsSuccess = true,
                Result = todoEntity
            });
        }

        [FunctionName(nameof(GetTodo))]
        public static async Task<IActionResult> GetTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo")] HttpRequest req,
            [Table("todo", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
            ILogger log)
        {
            log.LogInformation("Get received.");

            TableQuery<TodoEntity> query = new TableQuery<TodoEntity>();
            TableQuerySegment<TodoEntity> todos = await todoTable.ExecuteQuerySegmentedAsync(query, null);

            string message = $"Retrieved All Todos";
            log.LogInformation(message);

            return new ObjectResult(new Response
            {
                IsSuccess = true,
                Result = todos
            });
        }

        [FunctionName(nameof(GetTodoById))]
        public static IActionResult GetTodoById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo/{id}")] HttpRequest req,
            [Table("todo", "TODO", "{id}", Connection = "AzureWebJobsStorage")] TodoEntity todoEntity,
            string id,
            ILogger log)
        {
            log.LogInformation($"Get by Id: {id} received.");

            if (todoEntity == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "Todo not found."
                });
            }

            string message = $"Todo Id: {id} retrieved.";
            log.LogInformation(message);

            return new ObjectResult(new Response
            {
                IsSuccess = true,
                Result = todoEntity
            });
        }

        [FunctionName(nameof(UpdateTodo))]
        public static async Task<IActionResult> UpdateTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "todo/{id}")] HttpRequest req,
            [Table("todo", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
            string id,
            ILogger log)
        {
            log.LogInformation($"Recieved an update Todo for {id}");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Todo todo = JsonConvert.DeserializeObject<Todo>(requestBody);

            if(todo != null)
            {
                // Validate todo id
                TableOperation findOperation = TableOperation.Retrieve<TodoEntity>("TODO", id);
                TableResult findResult = await todoTable.ExecuteAsync(findOperation);

                if (findResult.Result == null)
                {
                    return new BadRequestObjectResult(new Response
                    {
                        IsSuccess = false,
                        Message = "Todo not found."
                    });
                }

                // Update todo
                TodoEntity todoEntity = (TodoEntity)findResult.Result;
                todoEntity.IsCompleted = todo.IsCompleted;

                if (!string.IsNullOrEmpty(todo?.TaskDescription))
                {
                    todoEntity.TaskDescription = todo.TaskDescription;
                }

                TableOperation updOperation = TableOperation.Replace(todoEntity);
                await todoTable.ExecuteAsync(updOperation);

                string message = $"Todo {id} updated on Table";
                log.LogInformation(message);

                return new ObjectResult(new Response
                {
                    IsSuccess = true,
                    Result = todoEntity
                });
            }

            return new BadRequestObjectResult(new Response
            {
                IsSuccess = false,
                Message = "Bad Request."
            });
        }
    }
}
