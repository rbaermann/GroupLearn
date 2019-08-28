using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GroupLearn.Models;

namespace GroupLearn.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;
        public HomeController(MyContext context)
        {
            dbContext = context;
        }
        
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("newUser")]
        public ViewResult NewUser()
        {
            CreatingUserViewModel viewModel = new CreatingUserViewModel();

            viewModel.ListOfSchools = dbContext.Schools
            .ToList();
            return View("NewUser", viewModel);
        }

        [HttpPost("register")]
        public IActionResult CreateUser(LoginViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.Users.Any(u=>u.Email == viewModel.newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use!");
                    return View("Index");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                viewModel.newUser.Password = Hasher.HashPassword(viewModel.newUser, viewModel.newUser.Password);

                dbContext.Users.Add(viewModel.newUser);
                dbContext.SaveChanges();

                HttpContext.Session.SetInt32("InSession", viewModel.newUser.UserId);

                return RedirectToAction("Dashboard");


            }
                else
                {
                    return View("Index");
                }
        }

        [HttpPost("login")]
        public IActionResult LoginUser(LoginViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                var dbUser = dbContext.Users.FirstOrDefault(u=>u.Email == viewModel.newLogin.loginEmail);
                if(dbUser == null)
                {
                    ModelState.AddModelError("newLogin.loginEmail", "Invalid Email");
                    return View("Index");
                }

                var hasher = new PasswordHasher<Login>();
                var result = hasher.VerifyHashedPassword(viewModel.newLogin, dbUser.Password, viewModel.newLogin.loginPassword);
                if(result == 0)
                {
                    ModelState.AddModelError("newLogin.loginPassword", "Password does not match email");
                    return View("Index");
                }
                HttpContext.Session.SetInt32("InSession", dbUser.UserId);

                return RedirectToAction("Dashboard");
            }
            else
            {
                return View("Index");
            }
        }

        [HttpPost("createSchool")]
        public RedirectToActionResult CreateSchool(CreatingUserViewModel submittedSchool)
        {
            School viewModel = submittedSchool.NewSchool;
            
            dbContext.Add(viewModel);
            dbContext.SaveChanges();
            return RedirectToAction("NewUser");
        }

        [HttpGet("/Dashboard")]
        public IActionResult Dashboard()
        {
            if(HttpContext.Session.GetInt32("InSession")!=null)
            {
                return View("Dashboard");
            }
            return RedirectToAction("Index");
        }
    }
}