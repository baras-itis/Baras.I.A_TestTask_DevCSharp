using Microsoft.AspNetCore.Mvc;
using Baras.Test.Models;
using Baras.Test.Services;

namespace Baras.Test.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScanController : ControllerBase
    {
        private readonly InMemoryStore _store;
        private readonly ScanQueue _queue;
        private readonly ILogger<ScanController> _logger;
        public ScanController(InMemoryStore store, ScanQueue queue, ILogger<ScanController> logger)
        {
            _store = store;
            _queue = queue;
            _logger = logger;
        }

        public record StartScanRequest(string RepoUrl);

        [HttpPost]
        public async Task<IActionResult> Start([FromBody] StartScanRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.RepoUrl))
            {
                _logger.LogInformation("Запрос с пустой ссылкой на репозиторий");
                return BadRequest(new { message = "Ссылка на репозиторий обязательна" });
            }
            var task = new ScanTask
            {
                Id = Guid.NewGuid(),
                RepoUrl = request.RepoUrl,
                Status = ScanStatus.Pending,
                LastStatusChange = DateTime.UtcNow
            };
            _store.ScanTasks.TryAdd(task.Id, task);
            await _queue.EnqueueAsync(task);
            _logger.LogInformation($"Начато сканирование репозитория {task.RepoUrl} /t {task.Id} ");
            return Ok(new { taskId = task.Id });

        }

        [HttpGet("{id}")]
        public IActionResult GetStatus(Guid id)
        {

            if (_store.ScanTasks.TryGetValue(id, out var task))
            {

                return Ok(new
                {
                    status = task.Status.ToString(),
                    lastStatusChange = task.LastStatusChange,
                    result = task.Result
                });
            }

            _logger.LogInformation("Задача не найдена");
            return NotFound(new { message = "Задача не найдена" });
        }
    }
}
