using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } // MongoDB จะใช้ ObjectId
    [BsonElement("username")]
    public string Username { get; set; }
    [BsonElement("password")]
    public string Password { get; set; } // เก็บรหัสผ่านที่ถูกแฮช
    [BsonElement("role")]
    public string Role { get; set; } // เช่น "Admin" หรือ "User"
    [BsonElement("phone")]
    public string Phone { get; set; } // หมายเลขโทรศัพท์
}