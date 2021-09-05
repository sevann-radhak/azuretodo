using System;
using System.Threading.Tasks;
using Azuretodo.Functions.Entities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;

namespace Azuretodo.Functions.Functions
{
    public static class AddTodosScheduledFunction
    {
        [FunctionName("AddTodosScheduled")]
        public static async Task Run(
            [TimerTrigger("0 */1 * * * *")]TimerInfo myTimer,
            [Table("todo", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
            ILogger log)
        {
            log.LogInformation($"Creating Task trigger function executed at: {DateTime.Now}");

            for (int i = 1; i < 11; i++)
            {
                TodoEntity todoEntity = new TodoEntity()
                {
                    CreatedTime = DateTime.UtcNow,
                    ETag = "*",
                    IsCompleted = false,
                    PartitionKey = "TODO",
                    RowKey = Guid.NewGuid().ToString(),
                    TaskDescription = $"Scheduled Task #{i}"
                };

                TableOperation addOperation = TableOperation.Insert(todoEntity);
                await todoTable.ExecuteAsync(addOperation);
            }

            log.LogInformation($"Creating Task trigger function ended at: {DateTime.Now}.");
        }
    }
}
