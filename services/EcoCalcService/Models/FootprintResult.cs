using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EcoCalcService.Models
{
    public class FootprintResult
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string UserId { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Category { get; set; } = null!;
        public DateTime Date { get; set; }
        public decimal CarbonFootprint { get; set; }
        public string? ExpenseId { get; set; }
    }
}