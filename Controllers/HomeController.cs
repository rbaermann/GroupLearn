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

        [HttpGet("User/{UserId}")]
        public IActionResult ViewUser(int UserId)
        {
            if(HttpContext.Session.GetInt32("InSession")!=null)
            {
                UserInfoViewModel viewModel = new UserInfoViewModel();

                viewModel.UserInfo = dbContext.Users
                .Include(u => u.UserRates)
                .FirstOrDefault(u => u.UserId == UserId);

                viewModel.CurrentUser = dbContext.Users
                .FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("InSession"));

                viewModel.UsersSchool = dbContext.Schools
                .FirstOrDefault(s => s.SchoolId == viewModel.UserInfo.SchoolId);

                return View("ViewUser", viewModel);
            }
            return RedirectToAction("Index");
        }

        [HttpPost("userRating/{UserId}")]
        public IActionResult UserRating(UserInfoViewModel submittedRating, int UserId)
        {
            UserRates newRate = new UserRates();
            
            newRate.UserId = UserId;
            newRate.CurUser = (int)HttpContext.Session.GetInt32("InSession");
            newRate.Rating = submittedRating.UserInfo.allRatings;
            
            dbContext.UserRates.Add(newRate);
            dbContext.SaveChanges();

            User user = dbContext.Users
            .Include(u => u.UserRates)
            .FirstOrDefault(u => u.UserId == UserId);

            user.allRatings += submittedRating.UserInfo.allRatings;
            user.Rating = (user.allRatings/user.UserRates.Count);
            dbContext.SaveChanges();

            return RedirectToAction("ViewUser", new{UserId = UserId});
        }

        [HttpGet("removeRating/{UserId}/{CurrentUserId}")]
        public RedirectToActionResult RemoveRating(int UserId, int CurrentUserId)
        {
            UserRates thisRate = dbContext.UserRates
            .FirstOrDefault(ur => ur.UserId == UserId && ur.CurUser == CurrentUserId);

            User user = dbContext.Users
            .Include(u => u.UserRates)
            .FirstOrDefault(u => u.UserId == UserId);

            dbContext.UserRates.Remove(thisRate);
            dbContext.SaveChanges();

            user.allRatings -= thisRate.Rating;if(user.UserRates.Count == 0)
            {
                user.Rating = 0;
            }
            else
            {
                user.Rating = (user.allRatings/user.UserRates.Count);
            }
            
            dbContext.SaveChanges();

            return RedirectToAction("ViewUser", new{UserId = UserId});
        }
    }
}