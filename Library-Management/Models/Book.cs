using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
public class Book
{
    internal string Username;

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
                                          // เพิ่มสถานะหนังสือ
    [BsonRepresentation(BsonType.String)]
    public BookStatus Status { get; set; } = BookStatus.Available; // ค่าเริ่มต้นเป็น Available
}
// Enum สำหรับสถานะหนังสือ
public enum BookStatus
{
    Available, // หนังสือที่พร้อมให้ยืม
    Borrowed   // หนังสือที่ถูกยืม
}