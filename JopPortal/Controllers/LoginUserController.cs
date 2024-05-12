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
using Microsoft.AspNetCore.Mvc.Rendering;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Linq;



namespace JopPortal.Controllers
{
    public class LoginUserController : Controller
    {
        private readonly AppDbContext context;
        private readonly IHostingEnvironment _Host;
        public LoginUserController(AppDbContext context, IHostingEnvironment Host)
        {
            this.context = context;
            _Host = Host;
        }


        public IActionResult Profile()
        {
            var Email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(Email))
            {
                return RedirectToAction("LogIn");
            }
            User user = context.user.FirstOrDefault(u => u.Email == Email);

            return View(user);
        }




        [HttpGet]
        public IActionResult LogIn()
        {
            return View(new User());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult LogInSave(User Us)
        {

            User user = context.user.FirstOrDefault(x => x.Email == Us.Email && x.Password == HashPassword(Us.Password));
            User isEmailExist = context.user.FirstOrDefault(x => x.Email == Us.Email);
            User isPassExist = context.user.FirstOrDefault(x => x.Password == HashPassword(Us.Password));


            if (user != null && user.Password == HashPassword(Us.Password))
            {
                HttpContext.Session.SetString("Email", Us.Email);
                //  ViewBag.IsAuthenticated = true;
                return RedirectToAction("Index", "Home");
            }
            else
            {


                ModelState.Clear();
                if (isEmailExist == null)
                {
                    ModelState.AddModelError("Email", "This Email is not found");
                    return View("LogIn", Us);

                }
                else
                {
                    ModelState.AddModelError("Password", "Incorrect Password");
                    //ViewBag.IsAuthenticated = false;
                    return View("LogIn", Us);
                }
            }
        }


        [HttpGet]
        public IActionResult SignUp()
        {

            return View(new User());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SignUpSave(User Us)
        {
            var isEmailExist = context.user.Any(x => x.Email == Us.Email);
            if (isEmailExist)
            {
                ModelState.AddModelError("Email", "Email is already exists");
                return View("SignUp", Us);
            }
            if (ModelState.IsValid)
            {
                if (Us.Password == Us.ConfirmPassword)
                {
                    Us.Password = HashPassword(Us.Password);
                    Us.ConfirmPassword = HashPassword(Us.ConfirmPassword);

                    string FileName = null;
                    if (Us.Photo != null)
                    {
                        string Saveroot = Path.Combine(_Host.WebRootPath, "images");
                        FileName = Us.Photo.FileName;
                        string Fullpath = Path.Combine(Saveroot, FileName);
                        Us.Photo.CopyTo(new FileStream(Fullpath, FileMode.Create));
                        Us.PhotoPath = FileName;
                    }
                    string FileName4 = null;
                    if (Us.File != null)
                    {
                        string Saveroot = Path.Combine(_Host.WebRootPath, "images");
                        FileName4 = Us.File.FileName;
                        string Fullpath = Path.Combine(Saveroot, FileName4);
                        Us.File.CopyTo(new FileStream(Fullpath, FileMode.Create));
                        Us.FilePath = FileName4;
                    }


                    context.user.Add(Us);
                    context.SaveChanges();
                    return RedirectToAction("LogIn");
                }
            }
            return View("SignUp", Us);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || context.user == null)
            {
                return NotFound();
            }

            var user = await context.user
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }


        public IActionResult Edit(int? id)
        {

            User user = context.user.FirstOrDefault(u => u.Id == id);
            return View(user);
        }
        public IActionResult SaveEdit(int id, User model, string currentPassword, string newPassword, string confirmNewPassword)
        {
            User user = context.user.FirstOrDefault(u => u.Id == id);
            string current = HashPassword(currentPassword);
            if (current == user.Password)
            {
                if (newPassword == confirmNewPassword)
                {
                    user.Name = model.Name;
                    user.Email = model.Email;
                    user.Password = HashPassword(model.Password);
                    user.ConfirmPassword = HashPassword(model.ConfirmPassword);
                    user.Age = model.Age;
                    user.Gender = model.Gender;
                    user.Address = model.Address;
                    user.PhoneNumber = model.PhoneNumber;
                    user.Photo = model.Photo;
                    // اضافة laple
                    //user.FilePath = model.FilePath;
                   

                    string FileName = null;

                    if (model.Photo != null)
                    {
                        string Saveroot = Path.Combine(_Host.WebRootPath, "images");
                        FileName = model.Photo.FileName;
                        string Fullpath = Path.Combine(Saveroot, FileName);
                        model.Photo.CopyTo(new FileStream(Fullpath, FileMode.Create));
                        model.PhotoPath = FileName;
                    }
                    user.PhotoPath = model.PhotoPath;

                    if (!string.IsNullOrEmpty(newPassword))
                    {
                        user.Password = HashPassword(newPassword);
                    }
                    context.SaveChanges();
                    return RedirectToAction("Profile");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "New password and confirm new password do not match.");
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

            var user = context.user.FirstOrDefault(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await context.user.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            context.user.Remove(user);
            await context.SaveChangesAsync();
            HttpContext.Session.Clear();
            return RedirectToAction("LogIn");
        }

        public async Task<IActionResult> MyApplications()
        {
            var Email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(Email))
            {
                return RedirectToAction("LogIn");
            }

            var user = await context.user.Include(u => u.ApplyJobs).ThenInclude(aj => aj.Job)
                           .FirstOrDefaultAsync(u => u.Email == Email);

            if (user == null)
            {
                return NotFound();
            }

            return View(user.ApplyJobs);
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
