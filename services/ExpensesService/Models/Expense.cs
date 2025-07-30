using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExpensesService.Models
{
    public class Expense
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string Category { get; set; } = null!;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }
    }
}