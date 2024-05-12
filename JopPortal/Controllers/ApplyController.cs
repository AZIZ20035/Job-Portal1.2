using jobPortal.Models;
using JopPortal.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JopPortal.Controllers
{
    public class ApplyController : Controller
    {
        private readonly AppDbContext context;
        public ApplyController(AppDbContext context)
        {
            this.context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ShowJob(int id)
        {
            var job = context.jop.FirstOrDefault(j => j.JobId == id);
            if (job == null)
            {
                return NotFound(); 
            }

            var Email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(Email))
            {
                return RedirectToAction("LogIn");
            }
            User user = context.user.FirstOrDefault(u => u.Email == Email);

            bool hasApplied = context.applyJop.Any(a => a.JobId == id && a.UserId == user.Id);

         
            ViewBag.HasApplied = hasApplied;

            return View(job); 
        }

        [HttpPost]
        public IActionResult ApplyForJob(int jobId)
        {
            
            var Email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(Email))
            {
                return RedirectToAction("LogIn");
            }
            User user = context.user.FirstOrDefault(u => u.Email == Email);

            var application = new ApplyJob
            {
                UserId = user.Id,
                JobId = jobId,
                File = user.File,
                FilePath = user.FilePath,
                State = State.Pending,
                Date = DateTime.Now
            };

            context.applyJop.Add(application);
            context.SaveChanges();

            return RedirectToAction("Explore_Jobs", "Home"); 
        }


        [HttpPost]
        public IActionResult CancelApplication(int jobId)
        {
            var Email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(Email))
            {
                return RedirectToAction("LogIn");
            }
            User user = context.user.FirstOrDefault(u => u.Email == Email);

            var application = context.applyJop.FirstOrDefault(a => a.JobId == jobId && a.UserId == user.Id);
            if (application == null)
            {
                return NotFound(); 
            }

            context.applyJop.Remove(application);
            context.SaveChanges();

            return RedirectToAction("Explore_Jobs", "Home");
        }
    }
}
