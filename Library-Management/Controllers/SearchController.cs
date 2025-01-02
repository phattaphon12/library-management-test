using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
public class SearchController : Controller
{
    private readonly MongoDbService _mongoDbService;
    public SearchController(MongoDbService mongoDbService)
    {
        _mongoDbService = mongoDbService;
    }
    // ฟังก์ชันค้นหาหนังสือ
    public IActionResult SearchBook(string search)
    {
        var books = string.IsNullOrEmpty(search)
            ? _mongoDbService.GetBooksCollection().Find(book => true).ToList()
            : _mongoDbService.GetBooksCollection().Find(book => book.Title.ToLower().Contains(search.ToLower())).ToList();
        ViewData["SearchQuery"] = search;
        return View(books); // ส่งข้อมูลหนังสือไปยัง View
    }
    // ฟังก์ชันดูรายละเอียดหนังสือ
    public IActionResult Details(string id)
    {
        var collection = _mongoDbService.GetBooksCollection();
        var book = collection.Find(b => b.Id == id).FirstOrDefault();
        if (book == null)
        {
            return NotFound();
        }
        return View(book); // ส่งข้อมูลรายละเอียดหนังสือไปยัง View
    }
}