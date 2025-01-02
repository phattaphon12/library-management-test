using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;
public class RegisterModel : PageModel
{
    private readonly MongoDbService _mongoDbService;
    public RegisterModel(MongoDbService mongoDbService)
    {
        _mongoDbService = mongoDbService;
    }
    [BindProperty]
    public string Username { get; set; }
    [BindProperty]
    public string Phone { get; set; }
    [BindProperty]
    public string Password { get; set; }
    [BindProperty]
    public string ConfirmPassword { get; set; }
    public async Task<IActionResult> OnPostAsync()
    {
        if (Password != ConfirmPassword)
        {
            ModelState.AddModelError(string.Empty, "Passwords do not match.");
            return Page();
        }
        var userCollection = _mongoDbService.GetUsersCollection();
        var existingUser = await userCollection.Find(u => u.Username == Username).FirstOrDefaultAsync();
        if (existingUser != null)
        {
            ModelState.AddModelError(string.Empty, "Username already exists.");
            return Page();
        }
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(Password);
        var newUser = new User
        {
            Username = Username,
            Phone = Phone,
            Password = hashedPassword,
            Role = "User"
        };
        await userCollection.InsertOneAsync(newUser);
        return RedirectToPage("Login"); // ไปยังหน้า Login หลังจากสมัครสมาชิกสำเร็จ
    }
}