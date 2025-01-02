using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
namespace LibraryManagement.Middleware
{
    public class UsernameMiddleware
    {
        private readonly RequestDelegate _next;
        public UsernameMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            // ดึงค่า Username จาก Session และตั้งค่าใน HttpContext.Items
            var username = context.Session.GetString("Username");
            context.Items["Username"] = username;
            // เรียก Middleware ถัดไปใน pipeline
            await _next(context);
        }
    }
}