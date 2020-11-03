using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moon.Entities;
using Moon.Models;
using Moon.SessionExtensions;
using PagedList.Core;

namespace Moon.Controllers
{

    public class HomeController : Controller
    {
        private readonly StudentContext _context;

        public HomeController(StudentContext context)
        {
            _context = context;
        }

        public IActionResult Index(string sortOrder,string currentFilter,string SearchCode,int? pageNumber, int SearchGroup)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentCourseFilter"] = SearchCode;
            ViewData["CurrentFilter"] = SearchGroup;
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
            if (!String.IsNullOrEmpty(SearchCode) && SearchGroup >= 100)
            {
                posts = posts.Where(s => s.CourseCode.Contains(SearchCode) && s.Category == SearchGroup);
            }
            if (!String.IsNullOrEmpty(SearchCode))
            {
                posts = posts.Where(s => s.CourseCode.Contains(SearchCode));
            }
            if (SearchCode != null)
            {
                pageNumber = 1;
            }
            else
            {
                SearchCode = currentFilter;
            }
            // giris yapilip yapilmamasina gore sayfa sekil alacak
            var obj = HttpContext.Session.GetObject<Student>("student");
            if (obj != null)
            {
                return RedirectToAction("Index", "Student");
            }
            int pageSize = 8;
            return View(posts.ToPagedList(pageNumber ?? 1, pageSize));
            // table contextinin kullanilabilmesi için yukarıda olusturulan nesne kullanıldı
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
        public IActionResult Login(Student student)
        {
            if (ModelState.IsValid)
            {
                var check = _context.Students.Where(std => std.id.Equals(student.id) && std.password.Equals(student.password)).ToList();
                if(check.Count > 0)
                {
                    // session baslatma
                    var bytes = Encoding.UTF8.GetBytes(student.id);
                    HttpContext.Session.Set("id", bytes);

                    var bytes2 = Encoding.UTF8.GetBytes(student.password);
                    HttpContext.Session.Set("password", bytes2);

                    HttpContext.Session.SetObject("student", student);
                    return Redirect("Index");
                }
            }
            
            return Redirect("Login");
        }

        public IActionResult Add()
        {
            ViewData["Message"] = "Your add page";

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(Student student)
        {
            if (ModelState.IsValid)
            {
                var check = _context.Students.FirstOrDefault(std => std.id == student.id);
                if (check == null)
                {
                    _context.Students.Add(student);
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.error = "User already exists";
                    return View();
                }
            }

            return View();
        }

        public IActionResult LogOut(Student student)
        {
            var obj = HttpContext.Session.GetObject<Student>("student");
            if (obj != null)
            {
                return RedirectToAction("LogOut", "Student");
            }
            // table contextinin kullanilabilmesi için yukarıda olusturulan nesne kullanıldı
            return View(_context.Files.ToList());
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
