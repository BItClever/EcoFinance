using MongoDB.Driver;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using EcoCalcService.Models;

namespace EcoCalcService.Services
{
    public class RabbitMqListener : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _config;

        public RabbitMqListener(IServiceProvider serviceProvider, IConfiguration config)
        {
            _serviceProvider = serviceProvider;
            _config = config;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _config["RabbitMq:Host"],
                Port = int.Parse(_config["RabbitMq:Port"] ?? "5672"),
                UserName = _config["RabbitMq:User"],
                Password = _config["RabbitMq:Password"]
            };

            using var connection = await factory.CreateConnectionAsync(); // Async!
            using var channel = await connection.CreateChannelAsync(); // Async!

            await channel.QueueDeclareAsync(
                queue: _config["RabbitMq:Queue"],
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new AsyncEventingBasicConsumer(channel); // AsyncEventingBasicConsumer!
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    var expenseEvent = JsonSerializer.Deserialize<ExpenseEvent>(message);
                    if (expenseEvent != null)
                    {
                        var result = CalculateFootprint(expenseEvent);

                        using var scope = _serviceProvider.CreateScope();
                        var mongoConfig = _config.GetSection("MongoDbSettings");
                        var mongoClient = new MongoClient(mongoConfig["ConnectionString"]);
                        var db = mongoClient.GetDatabase(mongoConfig["DatabaseName"]);
                        var results = db.GetCollection<FootprintResult>("FootprintResults");
                        await results.InsertOneAsync(result);
                    }
                }
                catch { /* error logging */ }

                return;
            };

            await channel.BasicConsumeAsync(
                queue: _config["RabbitMq:Queue"],
                autoAck: true,
                consumer: consumer);

            // Держим сервис живым
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        private FootprintResult CalculateFootprint(ExpenseEvent expense)
        {
            // Примитивный расчёт: коэффициент по категории
            var categoryFactors = new Dictionary<string, decimal>
            {
                ["transport"] = 0.5m,
                ["food"] = 0.2m,
                ["utilities"] = 0.3m
                // ...дополнить
            };
            decimal factor = categoryFactors.TryGetValue(expense.Category?.ToLower() ?? "", out var f) ? f : 0.1m;

            return new FootprintResult
            {
                UserId = expense.UserId,
                Amount = expense.Amount,
                Category = expense.Category,
                Date = expense.Date,
                CarbonFootprint = expense.Amount * factor,
                ExpenseId = expense.ExpenseId
            };
        }

        public class ExpenseEvent
        {
            public string? ExpenseId { get; set; }
            public string UserId { get; set; } = null!;
            public decimal Amount { get; set; }
            public string Category { get; set; } = null!;
            public DateTime Date { get; set; }
        }
    }
}