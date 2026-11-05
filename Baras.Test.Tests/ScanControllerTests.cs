using Baras.Test.Controllers;
using Baras.Test.Models;
using Baras.Test.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Baras.Test.Tests
{
    public class ScansControllerTests
    {
        [Fact]
        public async Task Start_ValidUrl_ReturnsTaskIdAndEnqueues()
        {
            var store = new InMemoryStore();
            var queue = new ScanQueue(); // Настоящая очередь
            var loggerMock = new Mock<ILogger<ScanController>>();
            var controller = new ScanController(store, queue, loggerMock.Object);

            var request = new ScanController.StartScanRequest("https://gitlab.com/test/repo");

            var result = await controller.Start(request);

            var okResult = Assert.IsType<OkObjectResult>(result);

            // Проверяем, что задача сохранилась в хранилище со статусом Pending
            var task = Assert.Single(store.ScanTasks.Values);
            Assert.Equal(ScanStatus.Pending, task.Status);
        }

        [Fact]
        public void GetStatus_NonExistingId_ReturnsNotFound()
        {
            var store = new InMemoryStore();
            var queue = new ScanQueue();
            var loggerMock = new Mock<ILogger<ScanController>>();
            var controller = new ScanController(store, queue, loggerMock.Object);

            var result = controller.GetStatus(Guid.NewGuid());

            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}