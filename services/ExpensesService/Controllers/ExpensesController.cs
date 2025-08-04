using ExpensesService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ExpensesService.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ExpensesController : ControllerBase
    {
        private readonly IMongoCollection<Expense> _expenses;

        public ExpensesController(IConfiguration config)
        {
            var client = new MongoClient(config["MongoDbSettings:ConnectionString"]);
            var database = client.GetDatabase(config["MongoDbSettings:DatabaseName"]);
            _expenses = database.GetCollection<Expense>("Expenses");
        }

        [HttpGet]
        public async Task<IEnumerable<Expense>> Get()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _expenses.Find(x => x.UserId == userId).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Expense>> Post(Expense expense)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            expense.UserId = userId;
            expense.Id = null;
            await _expenses.InsertOneAsync(expense);

            // Отправка события в RabbitMQ
            var factory = new ConnectionFactory()
            {
                HostName = Environment.GetEnvironmentVariable("RabbitMq__Host") ?? "eco_rabbitmq",
                Port = int.Parse(Environment.GetEnvironmentVariable("RabbitMq__Port") ?? "5672"),
                UserName = Environment.GetEnvironmentVariable("RabbitMq__User") ?? "guest",
                Password = Environment.GetEnvironmentVariable("RabbitMq__Password") ?? "guest"
            };

            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: "expenses",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var eventObj = new
            {
                ExpenseId = expense.Id,
                UserId = expense.UserId,
                Amount = expense.Amount,
                Category = expense.Category,
                Date = expense.Date
            };

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(eventObj));

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: "expenses",
                body: body);

            return CreatedAtAction(nameof(Get), new { id = expense.Id }, expense);
        }
    }
}