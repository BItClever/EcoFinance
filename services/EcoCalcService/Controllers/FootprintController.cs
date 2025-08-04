using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using EcoCalcService.Models;

namespace EcoCalcService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FootprintController : ControllerBase
    {
        private readonly IMongoCollection<FootprintResult> _results;

        public FootprintController(IConfiguration config)
        {
            var mongoConfig = config.GetSection("MongoDbSettings");
            var client = new MongoClient(mongoConfig["ConnectionString"]);
            var db = client.GetDatabase(mongoConfig["DatabaseName"]);
            _results = db.GetCollection<FootprintResult>("FootprintResults");
        }

        [HttpGet("{userId}")]
        public async Task<IEnumerable<FootprintResult>> Get(string userId)
            => await _results.Find(x => x.UserId == userId).ToListAsync();
    }
}