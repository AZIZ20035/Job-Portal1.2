using JopPortal.Data;
using JopPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace JopPortal.Controllers
{
    public class HomeController : Controller
    {
        /*private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }*/

        private readonly AppDbContext context;
        public HomeController(AppDbContext context)
        {
            this.context = context;
        }

        

        public IActionResult Index()
        {

            /*var Email = HttpContext.Session.GetString("Email");
            if(Email == null)
            {
                return RedirectToAction("LogIn", "LoginUser");
            }*/
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

        public IActionResult Explore_Jobs()
        {
            var Email = HttpContext.Session.GetString("Email");
            if (Email == null)
            {
                return RedirectToAction("LogIn", "LoginUser");
            }
            var jobs = context.jop.ToList();
            return View(jobs);
        }
    }
}
