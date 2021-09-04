using Azuretodo.Functions.Functions;
using Azuretodo.Test.Helpers;
using System;
using Xunit;

namespace Azuretodo.Test.Tests
{
    public class ScheduledFunctionTest
    {
        [Fact]
        public void ScheduledFunction_Should_Log_Messgae()
        {
            // Arrange
            MockCloudTableTodos mockTodos = new MockCloudTableTodos(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            ListLogger logger = (ListLogger)TestFactory.CreateLogger(LoggerTypes.List);
            // Act
            ScheduledFunction.Run(null, mockTodos, logger);
            string message = logger.Logs[0];

            // Assert
            Assert.Contains("Deleting completed", message);
        }
    }
}
