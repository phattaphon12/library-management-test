using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
public class Book
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Title { get; set; }
    public string Category { get; set; }
    public string Author { get; set; }
    public string Publisher { get; set; }
    [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
    public DateTime? PublishedDate { get; set; } // เปลี่ยนเป็น Nullable
    public string Isbn { get; set; }
    public string Description { get; set; }
    public string ImagePath { get; set; } // เก็บที่อยู่ของไฟล์รูปภาพ
}