using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
public class BooksController : Controller
{
    private readonly MongoDbService _mongoDbService;
    private BookStatus status;
    public BooksController(MongoDbService mongoDbService)
    {
        _mongoDbService = mongoDbService;
    }
    // แสดงหน้ารายการหนังสือทั้งหมด
    [Authorize(Roles = "Admin")]
    public IActionResult Index()
    {
        var books = _mongoDbService.GetBooksCollection().Find(book => true).ToList();
        return View(books);
    }

    // ค้นหาหนังสือตามชื่อ
    public IActionResult Search(string search)
    {
        // ถ้ามีคำค้นหา
        if (!string.IsNullOrEmpty(search))
        {
            var filter = Builders<Book>.Filter.Regex(
                book => book.Title,
                new MongoDB.Bson.BsonRegularExpression(search, "i") // ใช้ "i" สำหรับ ignore case
            );
            var books = _mongoDbService.GetBooksCollection()
                .Find(filter)
                .ToList();
            ViewBag.SearchQuery = search; // ส่งคำค้นหาไปยัง View
            return View("Index", books); // แสดงผลการค้นหาในหน้ารายการหนังสือ
        }
        // ถ้าไม่มีคำค้นหาก็แสดงรายการหนังสือทั้งหมด
        return RedirectToAction("Index");
    }

    [Authorize(Roles = "Admin")]
    public IActionResult AddBook()
    {
        return View();
    }
    [HttpPost]
    public IActionResult AddBook(IFormFile Image, Book book)
    {
        if (Image != null && Image.Length > 0)
        {
            // ระบุโฟลเดอร์สำหรับเก็บรูปภาพ
            var folderPath = Path.Combine("wwwroot", "images");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            // ตั้งชื่อไฟล์ใหม่เพื่อหลีกเลี่ยงชื่อซ้ำ
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Image.FileName);
            var filePath = Path.Combine(folderPath, fileName);
            // บันทึกไฟล์รูปภาพ
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                Image.CopyTo(stream);
            }
            // เก็บที่อยู่รูปภาพในฐานข้อมูล
            book.ImagePath = "/images/" + fileName;
        }
        // บันทึกข้อมูลหนังสือใน MongoDB
        var collection = _mongoDbService.GetBooksCollection();
        collection.InsertOne(book);
        return RedirectToAction("Index");
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Edit(string id)
    {
        var collection = _mongoDbService.GetBooksCollection();
        var book = collection.Find(b => b.Id == id).FirstOrDefault();
        if (book != null)
        {
            book.Status = status;
            collection.ReplaceOne(b => b.Id == id, book);
            return View(book);
        }
        return NotFound();
    }
    [HttpPost]
    public IActionResult Edit(Book updatedBook, IFormFile? Image)
    {
        var collection = _mongoDbService.GetBooksCollection();
        var book = collection.Find(b => b.Id == updatedBook.Id).FirstOrDefault();
        if (book == null)
        {
            return NotFound();
        }
        // อัปเดตข้อมูลรูปภาพใหม่ถ้ามีการอัปโหลด
        if (Image != null && Image.Length > 0)
        {
            var folderPath = Path.Combine("wwwroot", "images");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Image.FileName);
            var filePath = Path.Combine(folderPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                Image.CopyTo(stream);
            }
            updatedBook.ImagePath = "/images/" + fileName;
        }
        else
        {
            updatedBook.ImagePath = book.ImagePath; // ใช้รูปเดิมถ้าไม่มีการอัปโหลดใหม่
        }
        // อัปเดตข้อมูลใน MongoDB
        var updateDefinition = Builders<Book>.Update
            .Set(b => b.Title, updatedBook.Title)
            .Set(b => b.Category, updatedBook.Category)
            .Set(b => b.Author, updatedBook.Author)
            .Set(b => b.Publisher, updatedBook.Publisher)
            .Set(b => b.PublishedDate, updatedBook.PublishedDate)
            .Set(b => b.Isbn, updatedBook.Isbn)
            .Set(b => b.Description, updatedBook.Description)
            .Set(b => b.ImagePath, updatedBook.ImagePath);
        collection.UpdateOne(b => b.Id == updatedBook.Id, updateDefinition);
        return RedirectToAction("Index");
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult UpdateStatus(string id, BookStatus status)
    {
        var collection = _mongoDbService.GetBooksCollection();
        var book = collection.Find(b => b.Id == id).FirstOrDefault();
        if (book == null)
        {
            return NotFound();
        }
        // อัปเดตสถานะของหนังสือ
        book.Status = status;
        // อัปเดตข้อมูลใน MongoDB
        var updateDefinition = Builders<Book>.Update.Set(b => b.Status, status);
        collection.UpdateOne(b => b.Id == id, updateDefinition);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Delete(string id)
    {
        var collection = _mongoDbService.GetBooksCollection();
        var result = collection.DeleteOne(b => b.Id == id);
        if (result.DeletedCount == 0)
        {
            return Json(new { success = false, message = "ไม่สามารถลบหนังสือได้" });
        }
        return Json(new { success = true });
    }
}