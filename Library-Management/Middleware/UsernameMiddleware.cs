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
            // ดึงค่า Username และ Role จาก Session
            var username = context.Session.GetString("Username");
            var role = context.Session.GetString("Role");

            // ตั้งค่าใน HttpContext.Items
            context.Items["Username"] = username;
            context.Items["Role"] = role;

            // เรียก Middleware ถัดไปใน pipeline
            await _next(context);
        }
    }
}
