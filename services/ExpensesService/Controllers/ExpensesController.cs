using ExpensesService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Security.Claims;

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
            expense.UserId = userId; // Присваиваем из токена!
            expense.Id = null;
            await _expenses.InsertOneAsync(expense);
            return CreatedAtAction(nameof(Get), new { id = expense.Id }, expense);
        }
    }
}