using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
public class LoanController : Controller
{
    private readonly MongoDbService _mongoDbService;
    public LoanController(MongoDbService mongoDbService)
    {
        _mongoDbService = mongoDbService;
    }
    // ฟังก์ชันสำหรับยืมหนังสือ
    [HttpPost]
    public IActionResult BorrowBook(string bookId, string username)
    {
        // ค้นหาหนังสือจากฐานข้อมูล
        var book = _mongoDbService.GetBooksCollection().Find(b => b.Id == bookId).FirstOrDefault();
        if (book != null && book.Status == BookStatus.Available)
        {
            // สร้างข้อมูลการยืม
            var loan = new Loan
            {
                Id = Guid.NewGuid().ToString(),
                BookId = bookId,
                Username = username,
                BorrowedDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(14),
                IsReturned = false
            };
            // บันทึกข้อมูลการยืมลงฐานข้อมูล
            _mongoDbService.GetLoansCollection().InsertOne(loan);
            // อัปเดตสถานะหนังสือเป็น Borrowed
            book.Status = BookStatus.Borrowed;
            _mongoDbService.GetBooksCollection().ReplaceOne(b => b.Id == bookId, book);
            return RedirectToAction("Details", "Search", new { id = bookId });
            //TempData["AlertMessage"] = $"{username} ได้ยืมหนังสือ {book.Title} ต้องมาคืนในวันที่ {loan.DueDate.ToString("yyyy-MM-dd")}";
            //return RedirectToAction("Details", "Book", new { id = bookId });
        }
        return BadRequest("ไม่สามารถยืมหนังสือได้");
    }
    // ฟังก์ชันสำหรับคืนหนังสือ
    [HttpPost]
    public IActionResult ReturnBook(string bookId, string username)
    {
        // ค้นหาการยืมที่ยังไม่ได้คืน
        var loan = _mongoDbService.GetLoansCollection()
                   .Find(l => l.BookId == bookId && l.Username == username && !l.IsReturned)
                   .FirstOrDefault();
        if (loan != null)
        {
            // ค้นหาหนังสือจากฐานข้อมูล
            var book = _mongoDbService.GetBooksCollection().Find(b => b.Id == bookId).FirstOrDefault();
            if (book != null)
            {
                // อัปเดตสถานะหนังสือเป็น Available
                book.Status = BookStatus.Available;
                _mongoDbService.GetBooksCollection().ReplaceOne(b => b.Id == bookId, book);
                // อัปเดตสถานะการคืน
                loan.IsReturned = true;
                loan.ReturnedDate = DateTime.Now;
                _mongoDbService.GetLoansCollection().ReplaceOne(l => l.Id == loan.Id, loan);
                return RedirectToAction("Details", "Search", new { id = bookId });
            }
        }
        return BadRequest("ไม่สามารถคืนหนังสือได้");
    }
}