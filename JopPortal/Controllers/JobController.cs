using jobPortal.Models;
using JopPortal.Data;
using JopPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JopPortal.Controllers
{
    public class JobController : Controller
    {
        readonly private AppDbContext context;
        public JobController(AppDbContext context) 
        {
            this.context = context;
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Job());
        }
        [HttpPost]
        public IActionResult Create(Job j)
        {
            ModelState.Remove("Company");
            if (ModelState.IsValid)
            {
                var Email = HttpContext.Session.GetString("CompanyEmail");
                var company = context.company.FirstOrDefault(x => x.CompanyEmail == Email);
                j.CompanyId = company.CompanyId;
                context.jop.Add(j);
                context.SaveChanges();
                return RedirectToAction("alljobs");
            }
            return View(j);
           
        }
        public IActionResult alljobs() 
        {
            var Email = HttpContext.Session.GetString("CompanyEmail");
            var company = context.company.FirstOrDefault(x => x.CompanyEmail == Email);
            List<Job> list = context.jop.ToList();
            List<Job> ans = new List<Job>();
            foreach(Job job in list)
            {
                if(job.CompanyId == company.CompanyId)
                    ans.Add(job);
            }
            return View(ans);
        }
        public IActionResult Details(int? id) 
        {
            var job = context.jop.FirstOrDefault(x => x.JobId == id);
            return View(job);
        }
        public IActionResult Edit(int? id)
        {
            Job job = context.jop.FirstOrDefault(c => c.JobId == id);
            return View(job);
        }

        [HttpPost]
        public IActionResult Edit(Job j)
        {
            Job job = context.jop.FirstOrDefault(c => c.JobId == j.JobId);
            job.JopLocation = j.JopLocation;
            job.JobCategory = j.JobCategory;
            job.JobDate = j.JobDate;
            job.JobDescription = j.JobDescription;
            job.JobStatus = j.JobStatus;
            job.AvailablePlaces = j.AvailablePlaces;
            job.JobType = j.JobType;
            job.JopSalary = j.JopSalary;
            context.SaveChanges();
            return RedirectToAction("alljobs");
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var job = context.jop.FirstOrDefault(x => x.JobId == id);
            return View(job);
        }
        [HttpPost]
        public IActionResult Delete(Job j)
        {
            var job = context.jop.FirstOrDefault(x => x.JobId == j.JobId);
            context.jop.Remove(job);
            context.SaveChanges();
            return RedirectToAction("alljobs");
        }
        public IActionResult ShowApplicants(int id)
        {
            
            var applicants = context.applyJop
                                    .Where(x => x.JobId == id) 
                                    .ToList();

            return View(applicants);
        }

        [HttpPost]
        public IActionResult AcceptApplication(int id)
        {
            var application = context.applyJop.FirstOrDefault(a => a.JobId == id);
            if (application != null)
            {
                application.State = State.Accepted;
                context.SaveChanges();
                // Redirect to a confirmation page or back to the list
                return RedirectToAction("alljobs"); // Adjust the redirection as needed
            }
            return NotFound();
        }

        [HttpPost]
        public IActionResult RejectApplication(int id)
        {
            var application = context.applyJop.FirstOrDefault(a => a.JobId == id);
            if (application != null)
            {
                application.State = State.Rejected;
                context.SaveChanges();
                // Redirect to a confirmation page or back to the list
                return RedirectToAction("alljobs"); // Adjust the redirection as needed
            }
            return NotFound();
        }
    }
}

