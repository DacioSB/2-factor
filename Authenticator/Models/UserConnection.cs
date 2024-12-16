using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Authenticator.Models
{
    public class UserConnection
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        [BsonElement("username")]
        public string Username { get; set; } = string.Empty;
        [BsonElement("issuer")]
        public string Issuer { get; set; } = string.Empty;
        [BsonElement("secret")]
        public string Secret { get; set; } = string.Empty;
        [BsonElement("createdAt")]
        public string CreatedAt { get; set; } = string.Empty;
        [BsonElement("updatedAt")]
        public string UpdatedAt { get; set; } = string.Empty;
        [BsonElement("totp")]
        public string totp { get; set; } = string.Empty;
        [BsonElement("future_totp")]
        public string futureTotp { get; set; } = string.Empty;
        [BsonElement("remainingSeconds")]
        public int remainingSeconds { get; set; } = 0;

    }
}