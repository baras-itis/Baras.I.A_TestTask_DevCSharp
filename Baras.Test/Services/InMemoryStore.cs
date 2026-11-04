using System.Collections.Concurrent;
using Baras.Test.Models;

namespace Baras.Test.Services
{
    public class InMemoryStore
    {
        // Используем потокобезопасную коллекцию, тк она гарантирует, что программа не упадет с ошибкой, когда потоки  
        // попытаются изменить или прочитать данные

        // Хранилище уязвимостей 
        public ConcurrentDictionary<Guid, Vulnerability> Vulnerabilities { get; } = new();
        // Хранилище сканированных задач
        public ConcurrentDictionary<Guid, ScanTask> ScanTasks { get; } = new();
    }
}
