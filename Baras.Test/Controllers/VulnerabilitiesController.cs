using Microsoft.AspNetCore.Mvc;
using Baras.Test.Models;
using Baras.Test.Services;

namespace Baras.Test.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VulnerabilitiesController : ControllerBase
    {
        private readonly InMemoryStore _store;
        // Интерфейс для логирвоания, для отслеживание всех запросов
        private readonly ILogger<VulnerabilitiesController> _logger;
        
        public VulnerabilitiesController(InMemoryStore store, ILogger<VulnerabilitiesController> logger)
        {
            _store = store;
            _logger = logger;   
        }

        public record CreateVulnerabilitiesRequest(string Name, SeverityLevel Severity);

        [HttpPost]
        public IActionResult Create([FromBody] CreateVulnerabilitiesRequest request)
        {
            _logger.LogInformation($"Запрос на создание записи об уязвимости \n :{request.Name}, {request.Severity} ");
            if (string.IsNullOrEmpty(request.Name))
            {
                _logger.LogInformation("Создание записи с пустым именем");
                return BadRequest(new { message = "Название не может быть пустым" });
            }

            var vulnerability = new Vulnerability
            {
                Id = new Guid(),
                Name = request.Name,
                Severity = request.Severity,
                CreatedAt = DateTime.UtcNow,
            };

            _store.Vulnerabilities.TryAdd(vulnerability.Id, vulnerability);
            _logger.LogInformation($" {vulnerability.CreatedAt} \n Уязвиость с id {vulnerability.Id} успешно создана");
            return Ok(new { id = vulnerability.Id });
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            _logger.LogInformation($"Поиск записи по id: \t {id}");
            if(_store.Vulnerabilities.TryGetValue(id, out var vulnerability))
            {
                return Ok(vulnerability);
            }
            _logger.LogWarning("Уязвимость с ID {Id} не найдена", id);
            return NotFound(new { message = "Не найдена уязвимость" });
        }
    }
}
