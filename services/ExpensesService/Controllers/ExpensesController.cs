using ExpensesService.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace ExpensesService.Controllers
{
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
        public async Task<IEnumerable<Expense>> Get() =>
            await _expenses.Find(_ => true).ToListAsync();

        [HttpPost]
        public async Task<ActionResult<Expense>> Post(Expense expense)
        {
            await _expenses.InsertOneAsync(expense);
            return CreatedAtAction(nameof(Get), new { id = expense.Id }, expense);
        }
    }
}