using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Authenticator.Models
{
    public class UserConnectionGetByIdDTO
    {
        [BsonElement("username")]
        public string Username { get; set; } = string.Empty;
        [BsonElement("issuer")]
        public string Issuer { get; set; } = string.Empty;
        [BsonElement("totp")]
        public string totp { get; set; } = string.Empty;
        [BsonElement("future_totp")]
        public string futureTotp { get; set; } = string.Empty;
        [BsonElement("remainingSeconds")]
        public int remainingSeconds { get; set; } = 0;
    }
}