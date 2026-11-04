
using Baras.Test.Services;

namespace Baras.Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));
            
            builder.Services.AddOpenApi();

            builder.Services.AddSwaggerGen();
               


            // регистрация наших сервисов
            builder.Services.AddSingleton<InMemoryStore>();
            builder.Services.AddSingleton<ScanQueue>();
            // регистрация фонового сервиса
            builder.Services.AddHostedService<ScanBackgroundWorker>();

            builder.Services.AddExceptionHandler<Baras.Test.Middleware.GlobalExceptionHandler>();
            // поддержка стандарта RFC 7807
            builder.Services.AddProblemDetails(); 

            var app = builder.Build();

            // включение глобального перехватчика ошибк
            app.UseExceptionHandler();

            // для вывода в консоль ссылки на документацию Swagger
            app.Lifetime.ApplicationStarted.Register(() =>
            {
                var serverAddresses = app.Services.GetService<Microsoft.AspNetCore.Hosting.Server.IServer>()?
                    .Features.Get<Microsoft.AspNetCore.Hosting.Server.Features.IServerAddressesFeature>()?
                    .Addresses;

                if(serverAddresses != null)
                {
                    var httpsAdress = serverAddresses.FirstOrDefault(a => a.StartsWith("https://"));
                    if(httpsAdress != null)
                    {
                        Console.WriteLine($"Сервер запущен на порту {httpsAdress}/swagger");
                    }
                }

            });

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/openapi/v1.json", "Baras.Test API v1");
                });
            }



            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
