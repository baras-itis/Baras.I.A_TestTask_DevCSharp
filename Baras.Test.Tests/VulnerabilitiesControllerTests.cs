using Baras.Test.Controllers;
using Baras.Test.Models;
using Baras.Test.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Baras.Test.Tests
{
    public class VulnerabilitiesControllerTests
    {
        private VulnerabilitiesController GetController(InMemoryStore store)
        {
            var loggerMock = new Mock<ILogger<VulnerabilitiesController>>();
            return new VulnerabilitiesController(store, loggerMock.Object);
        }

        [Fact]
        public void Create_ValidRequest_ReturnsOkAndSavesToStore()
        {
            var store = new InMemoryStore();
            var controller = GetController(store);
            var request = new VulnerabilitiesController.CreateVulnerabilitiesRequest("SQL Injection", SeverityLevel.High);
        
            var result = controller.Create(request);
            // проверяем что вернулся 200 код
            Assert.IsType<OkObjectResult>(result);
            Assert.Single(store.Vulnerabilities);
        }

        [Fact]
        public void Create_EmptyName_ReturnsBadRequest()
        {
            var store = new InMemoryStore();
            var controller = GetController(store);
            var request = new VulnerabilitiesController.CreateVulnerabilitiesRequest("", SeverityLevel.Low);
                
            var result = controller.Create(request);

            Assert.IsType<BadRequestObjectResult>(result);
            Assert.Empty(store.Vulnerabilities);
        }

        [Fact]
        public void GetById_ExistingId_ReturnsOk()
        {
            var store = new InMemoryStore();
            var controller = GetController(store);
            var vuln = new Vulnerability { Id = Guid.NewGuid(), Name = "Test", Severity = SeverityLevel.Critical };
            store.Vulnerabilities.TryAdd(vuln.Id, vuln);
            var result = controller.GetById(vuln.Id);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedVuln = Assert.IsType<Vulnerability>(okResult.Value);
            Assert.Equal(vuln.Id, returnedVuln.Id);
        }

        [Fact]
        public void GetById_NonExistingId_ReturnsNotFound()
        {
            var store = new InMemoryStore();
            var controller = GetController(store);

            var result = controller.GetById(Guid.NewGuid());
            
            Assert.IsType<NotFoundObjectResult>(result);
        }

    }
}
