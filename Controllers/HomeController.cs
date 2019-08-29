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
                GroupDashboardViewModel viewModel = new GroupDashboardViewModel();

                viewModel.currentGroups = dbContext.Groups
                    .Include(l=>l.Leader)
                    .Include(ug=>ug.UserGroups)
                    .ThenInclude(u=>u.User)
                    .ToList();

                viewModel.thisUser = dbContext.Users
                    .FirstOrDefault(u=>u.UserId==HttpContext.Session.GetInt32("InSession"));
                return View("Dashboard", viewModel);
            }
            return RedirectToAction("Index");
        }


        [HttpGet("/AddGroup")]
        public IActionResult AddGroup()
        {
            if(HttpContext.Session.GetInt32("InSession")!=null)
            {
                CreateGroupViewModel viewModel = new CreateGroupViewModel();
                viewModel.currentUser = dbContext.Users
                    .FirstOrDefault(u=>u.UserId==HttpContext.Session.GetInt32("InSession")); 
                return View("Create", viewModel);
            }
            return RedirectToAction("Index");
        }

        [HttpPost("/CreateGroup")]
        public IActionResult CreateGroup(CreateGroupViewModel viewModel)
        {
            System.Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!");
            if(HttpContext.Session.GetInt32("InSession")!=null)
            {
                System.Console.WriteLine("@@@@@@@@@@@@@@@@@@@@@@@@@");
                if(ModelState.IsValid)
                {
                    System.Console.WriteLine("###########################");
                    Group newGroup = viewModel.newGroup;
                    newGroup.Leader = dbContext.Users
                        .FirstOrDefault(u=>u.UserId==HttpContext.Session.GetInt32("InSession"));
                    viewModel.currentUser = dbContext.Users
                        .FirstOrDefault(u=>u.UserId==HttpContext.Session.GetInt32("InSession"));
                    newGroup.School = dbContext.Schools
                        .FirstOrDefault(s=>s.SchoolId == viewModel.currentUser.SchoolId);

                    dbContext.Add(newGroup);
                    dbContext.SaveChanges();
                    return RedirectToAction("Dashboard", newGroup);
                }
                System.Console.WriteLine("$$$$$$$$$$$$$$$$$$$$$$$$$$$");
                return View("Create");
            }
            return RedirectToAction("Index");
        }

        [HttpGet("Join/{GroupId}/{UserId}")]
        public IActionResult JoinGroup(int GroupId, int UserId)
        {
            if(HttpContext.Session.GetInt32("InSession")!=null)
            {
                UserGroup userGroup = new UserGroup();
                userGroup.UserId = UserId;
                userGroup.GroupId = GroupId;
                dbContext.UserGroups.Add(userGroup);
                dbContext.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            return RedirectToAction("Index");
        }

        [HttpGet("Remove/{GroupId}/{UserId}")]
        public IActionResult LeaveGroup(int GroupId, int UserId)
        {
            if(HttpContext.Session.GetInt32("InSession")!=null)
            {
                UserGroup userGroup = dbContext.UserGroups
                    .Where(ug=>ug.GroupId == GroupId && ug.UserId == UserId)
                    .FirstOrDefault();
                dbContext.UserGroups.Remove(userGroup);
                dbContext.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            return RedirectToAction("Index");
        }

        [HttpGet("Delete/{GroupId}")]
        public IActionResult StudyGroup(int GroupId)
        {
            if(HttpContext.Session.GetInt32("InSession")!=null)
            {
                Group group = dbContext.Groups
                    .Include(l=>l.Leader)
                    .FirstOrDefault(g=>g.GroupId==GroupId);

                if(group.Leader.UserId!= HttpContext.Session.GetInt32("InSession"))
                {
                    return RedirectToAction("Index");
                }

                dbContext.Groups.Remove(group);
                dbContext.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            return RedirectToAction("Index");

        }

        [HttpGet("ViewGroup/{GroupId}")]
        public IActionResult ViewGroup(int GroupId)
        {
            if(HttpContext.Session.GetInt32("InSession")!=null)
            {
                ViewGroupViewModel viewModel = new ViewGroupViewModel();
                viewModel.thisGroup = dbContext.Groups
                    .Include(l=>l.Leader)
                    .Include(ug=>ug.UserGroups)
                    .ThenInclude(u=>u.User)
                    .FirstOrDefault(g=>g.GroupId==GroupId);

                viewModel.thisUser = dbContext.Users
                    .FirstOrDefault(u=>u.UserId==HttpContext.Session.GetInt32("InSession"));
                return View("ViewGroup", viewModel);
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

                viewModel.UsersGroups = dbContext.Groups
                .Include(l=>l.Leader)
                .Include(ug=>ug.UserGroups)
                .ThenInclude(u=>u.User)
                .ToList();

                return View("ViewUser", viewModel);
            }
            return RedirectToAction("Index");
        }



        [HttpGet("/Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
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