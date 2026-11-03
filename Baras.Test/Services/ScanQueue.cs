using System.Threading.Channels;
using Baras.Test.Models;

namespace Baras.Test.Services
{
    public class ScanQueue
    {
        // создаем очередь для защиты от перегрузок 
        private readonly Channel<ScanTask> _queue;
        
        public ScanQueue()
        {
            // Создаем неограниченную очередь, чтобы класть любое количество задач
            _queue = Channel.CreateUnbounded<ScanTask>();
        }

        // добавление задач в очередь
        public async ValueTask EnqueueAsync(ScanTask task)
        {
            await _queue.Writer.WriteAsync(task);
        }

        public async ValueTask<ScanTask> DequeueAsync(CancellationToken cancellationToken)
        {
            return await _queue.Reader.ReadAsync(cancellationToken);
        }
    }
}
