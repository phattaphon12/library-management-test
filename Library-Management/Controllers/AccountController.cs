﻿using Library_Management.DTOs;
using Library_Management.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using BCrypt.Net;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace Library_Management.Controllers
{ //test github
    public class AccountController : Controller
    {
        private readonly IMongoCollection<User> _userCollection;
        private readonly IHttpContextAccessor _httpContextAccessor;
        // ปรับ Constructor ให้รับ IHttpContextAccessor
        public AccountController(MongoDbService mongoDbService, IHttpContextAccessor httpContextAccessor)
        {
            _userCollection = mongoDbService.GetUsersCollection();
            _httpContextAccessor = httpContextAccessor;  // กำหนดค่า IHttpContextAccessor
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(DTOs.RegisterRequest request)
        {
            // ตรวจสอบว่าผู้ใช้มีอยู่ในระบบแล้วหรือไม่
            var existingUser = await _userCollection.Find(u => u.Username == request.Username).FirstOrDefaultAsync();
            if (existingUser != null)
            {
                ModelState.AddModelError("", "Username already exists.");
                return View();
            }

            // แฮชรหัสผ่าน
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // สร้าง User ใหม่
            var newUser = new User
            {
                Username = request.Username,
                Password = hashedPassword,
                Role = request.Role,
                Phone = request.Phone
            };

            // บันทึกลง MongoDB
            await _userCollection.InsertOneAsync(newUser);

            return RedirectToAction("Login");
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(DTOs.LoginRequest request)
        {
            // ค้นหาผู้ใช้จากฐานข้อมูล
            var user = await _userCollection.Find(u => u.Username == request.Username).FirstOrDefaultAsync();
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                ModelState.AddModelError("", "Invalid credentials.");
                return View();
            }

            // สร้าง Claims สำหรับ Authentication
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Role, user.Role) // เพิ่ม Role ลงใน Claims
    };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true, // จดจำการเข้าสู่ระบบ (optional)
                ExpiresUtc = DateTime.UtcNow.AddMinutes(30) // ตั้งเวลา Session หมดอายุ
            };
            // ลงชื่อเข้าใช้ (Sign-in)
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
            // ตั้งค่า Session
            HttpContext.Session.SetString("Username", user.Username);
            // Redirect ตาม Role
            if (user.Role == "Admin")
            {
                return RedirectToAction("Index", "Books");
            }

            return RedirectToAction("SearchBook", "Search");
        }

        // GET: /Account/Logout
        // AccountController.cs
        [HttpPost]
        public IActionResult Logout()
        {
            // ลบ Cookies ที่เกี่ยวข้อง
            Response.Cookies.Delete(".AspNetCore.Cookies"); // ตัวอย่างสำหรับ Identity Cookie

            // Clear Session (ถ้าใช้ Session)
            HttpContext.Session.Clear();

            // Redirect ไปยังหน้า Login หรือหน้าแรก
            return RedirectToAction("Login");
        }
    }
}