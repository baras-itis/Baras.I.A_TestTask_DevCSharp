
using Baras.Test.Services;

namespace Baras.Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            // регистрация наших сервисов
            builder.Services.AddSingleton<InMemoryStore>();
            builder.Services.AddSingleton<ScanQueue>();
            // регистрация фонового сервиса
            builder.Services.AddHostedService<ScanBackgroundWorker>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
