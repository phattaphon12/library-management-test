using System.Diagnostics;
using Library_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;  // ????? namespace ???????

namespace Library_Management.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;  // ??????????? IHttpContextAccessor

        public HomeController(ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;  // ???????? IHttpContextAccessor
        }

        public IActionResult Index()
        {
            // ??? _httpContextAccessor ???????????? HttpContext.Session
            var username = _httpContextAccessor.HttpContext.Session.GetString("Username");
            ViewData["Username"] = username;  // ?????????????????? ViewData
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
