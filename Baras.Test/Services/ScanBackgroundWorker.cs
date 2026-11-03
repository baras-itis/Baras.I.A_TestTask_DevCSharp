
using Baras.Test.Models;

namespace Baras.Test.Services
{
    public class ScanBackgroundWorker : BackgroundService
    {
        private readonly ScanQueue _queue;
        private readonly InMemoryStore _store;
        // nen
        private readonly ILogger<ScanBackgroundWorker> _logger;

        public ScanBackgroundWorker(ScanQueue queue, InMemoryStore store, ILogger<ScanBackgroundWorker> logger)
        {
            _queue = queue; 
            _store = store;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Фоновый сервис запущен");

            // Для бесконечного цикла, пока приложение запущено
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Ожидаем задачу,  если очередь пуста поток будет жить тут
                    var task = await _queue.DequeueAsync(stoppingToken);
                    _ = ProcessTaskAsync(task, stoppingToken);
                }
                catch(OperationCanceledException)
                {
                    // при выключение сервера
                    break;
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при получении задачи из очереди");
                }
            }
        }

        private async Task ProcessTaskAsync(ScanTask task,  CancellationToken stoppingToken )
        {
            try
            {
                _logger.LogInformation($"Начато сканирвоание репозитория {task.RepoUrl}");

                task.Status = ScanStatus.Running;
                task.LastStatusChange = DateTime.UtcNow;

                // Имитация сканирования репозитория 
                await Task.Delay(5000, stoppingToken);

                task.Result = "Анализ завершен. Уязвимостей не обнаружено";
                task.Status = ScanStatus.Completed;
                task.LastStatusChange = DateTime.UtcNow;

                _logger.LogInformation($"Сканирование репозитория: {task.RepoUrl} завершено");
            } catch(Exception ex)
            {
                task.Status = ScanStatus.Error;
                task.Result = $"Ошибка при сканировании: {ex.Message}";
                task.LastStatusChange = DateTime.UtcNow;

                _logger.LogInformation($"{ex} Ошибка при сконировании репозитория {task.RepoUrl}");
            }
        }

    }
}
