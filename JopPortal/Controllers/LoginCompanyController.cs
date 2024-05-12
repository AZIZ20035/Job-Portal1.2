using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JopPortal.Data;
using JopPortal.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using jobPortal.Models;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace JopPortal.Controllers
{
    public class LoginCompanyController : Controller
    {
        private readonly AppDbContext context;
        private readonly IHostingEnvironment _Host;
        public LoginCompanyController(AppDbContext context, IHostingEnvironment Host)
        {
            this.context = context;
            _Host = Host;
        }
        public IActionResult Profile()
        {
            var Email = HttpContext.Session.GetString("CompanyEmail");
            if (string.IsNullOrEmpty(Email))
            {
                return RedirectToAction("LogIn");
            }
            Company company = context.company.FirstOrDefault(u => u.CompanyEmail == Email);

            return View(company);
        }




        [HttpGet]
        public IActionResult LogIn()
        {
            return View(new Company());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult LogInSave(Company Cp)
        {
            Company company = context.company.FirstOrDefault(x => x.CompanyEmail == Cp.CompanyEmail && x.CompanyPassword == HashPassword(Cp.CompanyPassword));//check if email and password are match
            Company isEmailExist = context.company.FirstOrDefault(x => x.CompanyEmail == Cp.CompanyEmail);
            Company isPassExist = context.company.FirstOrDefault(x => x.CompanyPassword == HashPassword(Cp.CompanyPassword));

            if (company != null)
            {
                HttpContext.Session.SetString("CompanyEmail", Cp.CompanyEmail);
                // ViewBag.IsAuthenticated = true;
                return RedirectToAction("Index", "CompanyHome");

            }
            else
            {
                ModelState.Clear();
                if (isEmailExist == null)
                {
                    ModelState.AddModelError("CompanyEmail", "This Email is not found");
                    return View("LogIn", Cp);
                }

                else
                {
                    ModelState.AddModelError("CompanyPassword", "Incorrect Password");
                    // ViewBag.IsAuthenticated = false;
                    return View("LogIn", Cp);
                }
            }

        }


        [HttpGet]
        public IActionResult SignUp()
        {

            return View(new Company());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SignUpSave(Company Cp)
        {
            var isEmailExist = context.company.Any(x => x.CompanyEmail == Cp.CompanyEmail);
            if (isEmailExist)
            {
                ModelState.AddModelError("CompanyEmail", "Email is already exists");
                return View("SignUp", Cp);
            }

            if (ModelState.IsValid)
            {
                if (Cp.CompanyPassword == Cp.ConfirmPassword)
                {
                    Cp.CompanyPassword = HashPassword(Cp.CompanyPassword);
                    Cp.ConfirmPassword = HashPassword(Cp.ConfirmPassword);

                    string FileName = null;
                    if (Cp.CompanyPhoto != null)
                    {
                        string Saveroot = Path.Combine(_Host.WebRootPath, "images");
                        FileName = Cp.CompanyPhoto.FileName;
                        string Fullpath = Path.Combine(Saveroot, FileName);
                        Cp.CompanyPhoto.CopyTo(new FileStream(Fullpath, FileMode.Create));
                        Cp.PhotoPath = FileName;
                    }

                    context.company.Add(Cp);
                    context.SaveChanges();
                    return RedirectToAction("LogIn");
                }
            }
            return View("SignUp", Cp);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || context.company == null)
            {
                return NotFound();
            }

            var company = await context.company
                .FirstOrDefaultAsync(m => m.CompanyId == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        public IActionResult Edit(int? id)
        {
            Company company = context.company.FirstOrDefault(c => c.CompanyId == id);
            return View(company);
        }

        [HttpPost]
        public IActionResult SaveEdit(Company model, string newPassword, string confirmPassword)
        {
            Company company = context.company.FirstOrDefault(c => c.CompanyId == model.CompanyId);
            if (HashPassword(model.CompanyPassword) == company.CompanyPassword)
            {
                if (newPassword == confirmPassword)
                {
                    company.CompanyName = model.CompanyName;
                    company.CompanyEmail = model.CompanyEmail;
                    company.CompanyPassword = HashPassword(newPassword);
                    company.ConfirmPassword = HashPassword(newPassword);
                    company.CompanyDescription = model.CompanyDescription;
                    company.PhotoPath = model.PhotoPath;


                    context.SaveChanges();
                    return RedirectToAction("Profile");
                }
                else
                {
                    ViewBag.Confirm = "New password and confirm new password do not match.";
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Current password is incorrect.");
            }

            return View("Edit", model);
        }

        public IActionResult DeleteConfirmation(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comp = context.company.FirstOrDefault(m => m.CompanyId == id);
            if (comp == null)
            {
                return NotFound();
            }

            return View(comp);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comp = await context.company.FindAsync(id);
            if (comp == null)
            {
                return NotFound();
            }
            context.company.Remove(comp);
            await context.SaveChangesAsync();
            HttpContext.Session.Clear();
            return RedirectToAction("LogIn");
        }




        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }
}
