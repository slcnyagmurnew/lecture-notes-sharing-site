using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moon.Entities;
using Moon.Models;
using Moon.SessionExtensions;
using PagedList.Core;
using Microsoft.AspNetCore.Identity.UI.Services;
using Moon_.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using Moon_.Entities;

namespace Moon.Controllers
{

    public class HomeController : Controller
    {
        private readonly StudentContext _context;
        private UserManager<IdentityUser> _userManager;
        private SignInManager<IdentityUser> _signInManager;
        private IEmailSender _emailSender;
        private RoleManager<IdentityRole> _roleManager;

        readonly JsonDataHelper _dataHelper = new JsonDataHelper();

        public HomeController(StudentContext context, UserManager<IdentityUser> userManager, 
            SignInManager<IdentityUser> signInManager, IEmailSender emailSender, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        public IActionResult Index(string sortOrder, string currentFilter, string SearchCode, int? pageNumber, string GroupValue)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentFilter"] = SearchCode;
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            var posts = from s in _context.Files
                        select s;
            switch (sortOrder)
            {
                case "Date":
                    posts = posts.OrderBy(s => s.CreatedOn);
                    break;
                case "date_desc":
                    posts = posts.OrderByDescending(s => s.CreatedOn);
                    break;
                default:
                    posts = posts.OrderByDescending(s => s.CreatedOn);
                    break;
            }
            if (!String.IsNullOrEmpty(SearchCode) && !String.IsNullOrEmpty(GroupValue))
            {
                posts = posts.Where(s => s.CourseCode.Equals(SearchCode) && s.Category.Equals(GroupValue));
            }
            else if (!String.IsNullOrEmpty(SearchCode))
            {
                posts = posts.Where(s => s.CourseCode.Equals(SearchCode));
            }
            else { }
            if (SearchCode != null)
            {
                pageNumber = 1;
            }
            else
            {
                SearchCode = currentFilter;
            }
            int pageSize = 8;

            ViewBag.CourseCode = new SelectList(_dataHelper.GetDict().Keys.ToList());
            var obj = HttpContext.Session.GetObject<Student>("student");
            if (obj != null)
            {
                return RedirectToAction("Index", "Student");
            }
            return View(posts.ToPagedList(pageNumber ?? 1, pageSize));
            // table contextinin kullanilabilmesi için yukarıda olusturulan nesne kullanıldı
        }

        [HttpPost]
        public IActionResult Index(IFormCollection form)
        {
            var optionValue = form["CourseCodeDrop"];
            var optionCategory = form["GroupValue"];
            return RedirectToAction("Index", new { SearchCode = optionValue, GroupValue = optionCategory });
        }

        public JsonResult CourseCategoryDrop(string id)
        {

            string value = null;
            if (_dataHelper.GetDict().ContainsKey(id))
            {
                value = _dataHelper.GetDict()[id];
            }
            List<string> SelectedCategories = value.Split(",").ToList();
            return Json(SelectedCategories);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Login()
        {
            ViewData["Message"] = "Your login page";
            
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Student student)
        {
            StudentViewModel model = new StudentViewModel();
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(student.Id);

                if(user == null)
                {
                    ModelState.AddModelError("", "User does not exists, please create new account.");
                    return View(model);
                }

                if(! await _userManager.IsEmailConfirmedAsync(user))
                {
                    ModelState.AddModelError("", "Please confirm your account.");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(user, student.Password, false, false);

                if (result.Succeeded)
                {
                    var bytes = Encoding.UTF8.GetBytes(student.Id);
                    HttpContext.Session.Set("id", bytes);

                    var bytes2 = Encoding.UTF8.GetBytes(student.Password);
                    HttpContext.Session.Set("password", bytes2);

                    HttpContext.Session.SetObject("student", student);
                    return RedirectToAction("Index", "Student");
                }
            }
            ModelState.AddModelError("", "Username or password invalid");
            return View(model);
        }

        public IActionResult Register()
        {
            ViewData["Message"] = "Your add page";

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Student student)
        {
            RegisterViewModel model = new RegisterViewModel();
            if (ModelState.IsValid)
            {
                var user = new Student()
                {
                    Name = student.Name,
                    Surname = student.Surname,
                    Id = student.Id,
                    UserName = student.Id,
                    Department = student.Department,
                    Email = student.Email,
                    Password = student.Password,
                    EmailConfirmed = false
                };

                var check = await _userManager.CreateAsync(user, student.Password);
                if (check.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Student");
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var url = Url.Action("ConfirmEmail", "Home", new
                    {
                        userId = user.Id,
                        token = code
                    });
                    await _emailSender.SendEmailAsync(user.Email, "Please confirm your account",$"Click to <a href='https://localhost:44392{url}'>link</a> for confirmation");
                    
                    return RedirectToAction("Login","Home");
                }
                ModelState.AddModelError("", "This mail already exists...");
            }

            return View(model);
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

        public async Task<ActionResult> ConfirmEmail(string userId, string token)
        {
            if(userId == null || token == null)
            {
                TempData["Message"] = "Unvalid token";
                return View();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if(user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    TempData["Message"] = "Account has been approved";
                    return View();
                }
            }
            TempData["Message"] = "No user found";
            return View();
        }
    }
}

