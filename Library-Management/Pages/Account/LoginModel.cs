using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;
public class LoginModel : PageModel
{
    private readonly MongoDbService _mongoDbService;
    public LoginModel(MongoDbService mongoDbService)
    {
        _mongoDbService = mongoDbService;
    }
    [BindProperty]
    public string Username { get; set; }
    [BindProperty]
    public string Password { get; set; }
    public async Task<IActionResult> OnPostAsync()
    {
        var userCollection = _mongoDbService.GetUsersCollection();
        var user = await userCollection.Find(u => u.Username == Username).FirstOrDefaultAsync();
        if (user == null || !BCrypt.Net.BCrypt.Verify(Password, user.Password))
        {
            ModelState.AddModelError(string.Empty, "Invalid credentials.");
            return Page();
        }
        // Set session or token here
        return RedirectToPage("Index"); // ไปยังหน้าแรกหลังจากล็อกอินสำเร็จ
    }
}