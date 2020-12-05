using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Moon.Entities;
using Moon.Models;
using Moon.SessionExtensions;
using Moon_.Entities;
using Moon_.Models;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace Moon.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {

        private readonly StudentContext _context;

        public StudentController(StudentContext context)
        {
            _context = context;
        }

        readonly JsonDataHelper _dataHelper = new JsonDataHelper();

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

            IPagedList<Files> _posts = posts.ToPagedList(pageNumber ?? 1, pageSize);
            List<Files> _favoriteList = Get_Favorite_Posts();

            var modelView = new ComplexModel(_posts, _favoriteList);

            return View(modelView);
            // table contextinin kullanilabilmesi için yukarıda olusturulan nesne kullanıldı
        }

        [HttpPost]
        public IActionResult Index(IFormCollection form)
        {
            var optionValue = form["CourseCodeDrop"];
            var optionCategory = form["GroupValue"];
            return RedirectToAction("Index", new { SearchCode = optionValue, GroupValue = optionCategory });
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

        public IActionResult LogOut(string id)
        {
            // alert = logged out successfully
            HttpContext.Session.Clear();
            return RedirectToAction("Index","Home");
        }
        // dokuman sifreleme icin, random degerler, filesta documentId ve poststa id ayni
        static String EncodeInt32AsString(Int32 input, Int32 maxLength = 0)
        {
            // List of characters allowed in the target string 
            Char[] allowedList = new Char[] {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J',
            'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T',
            'U', 'V', 'W', 'X', 'Y', 'Z' };
            Int32 allowedSize = allowedList.Length;
            StringBuilder result = new StringBuilder(input.ToString().Length);

            Int32 moduloResult;
            while (input > 0)
            {
                moduloResult = input % allowedSize;
                input /= allowedSize;
                result.Insert(0, allowedList[moduloResult]);
            }

            if (maxLength > result.Length)
            {
                result.Insert(0, new String(allowedList[0], maxLength - result.Length));
            }

            if (maxLength > 0)
                return result.ToString().Substring(0, maxLength);
            else
                return result.ToString();
        }

        static String GetRandomizedString(Int32 input)
        {
            Int32 uniqueLength = 6; // Length of the unique string (based on the input) 
            Int32 randomLength = 4; // Length of the random string (based on the RNG) 
            String uniqueString;
            String randomString;
            StringBuilder resultString = new StringBuilder(uniqueLength + randomLength);
            Random randomizer = new Random(
                    (Int32)(
                        DateTime.Now.Ticks + (DateTime.Now.Ticks > input ? DateTime.Now.Ticks / (input + 1) : input / DateTime.Now.Ticks)
                    )
                );
            randomString = EncodeInt32AsString(randomizer.Next(1, Int32.MaxValue), randomLength);
            uniqueString = EncodeInt32AsString(input, uniqueLength);

            resultString.AppendFormat("{0}{1}", uniqueString, randomString);
            for (Int32 i = 0; i < Math.Min(uniqueLength, randomLength); i++)
            {
                resultString.AppendFormat("{0}{1}", uniqueString[i], randomString[i]);
            }
            resultString.Append((uniqueLength < randomLength ? randomString : uniqueString).Substring(Math.Min(uniqueLength, randomLength)));

            return resultString.ToString();
        }

        public IActionResult DisplayPDF(string id)
        {
            byte[] byteArray = GetPdfFromDB(id);
            MemoryStream pdfStream = new MemoryStream();
            pdfStream.Write(byteArray, 0, byteArray.Length);
            pdfStream.Position = 0;
            // string showRule = "application/";
            // showRule += getFile.FileType.Replace(".", string.Empty);
            return new FileStreamResult(pdfStream, "application/pdf");
        }

        private byte[] GetPdfFromDB(string id)
        {
            #region
            byte[] bytes = { };
            string constr = GetConnectionString();
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "SELECT DataFiles FROM Files WHERE DocumentId=@Id";
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.HasRows == true)
                        {
                            sdr.Read();
                            bytes = (byte[])sdr["DataFiles"];
                        }
                    }
                    con.Close();
                }
            }

            return bytes;
            #endregion
        }

        static private string GetConnectionString()
        {
            // To avoid storing the connection string in your code,
            // you can retrieve it from a configuration file.
            return "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=StudentDb;Integrated Security=True;Pooling=False;"
                + "Integrated Security=true;";
        }

        public IActionResult MyCourses()
        {
            var posts = from s in _context.Files
                        select s;
            var owner = HttpContext.Session.GetObject<Student>("student");
            posts = posts.Where(s => s.ownerId.Equals(owner.Id));
            return View(posts.AsNoTracking());
        }

        public IActionResult Create()
        {
            ViewBag.CourseCode = new SelectList(_dataHelper.GetDict().Keys.ToList());
            ViewBag.Category = new SelectList(_dataHelper.GetDict().Values.ToList());
            return View();
        }
        [HttpPost]
        public IActionResult Create(IFormFile formFile, Files post)
        {
            if (formFile != null)
            {
                if (formFile.Length > 0)
                {
                    // dosya adini alma
                    var fileName = Path.GetFileName(formFile.FileName);
                    // dosya uzantisi
                    var fileExtension = Path.GetExtension(fileName);
                    // uzanti arti dosya adi
                    var newFileName = String.Concat(Convert.ToString(Guid.NewGuid()), fileExtension);
                    // yukleyen ogrenci id si anlik session ile aliniyor
                    var owner = HttpContext.Session.GetObject<Student>("student");

                    var objformFile = new Files()
                    {
                        DocumentId = GetRandomizedString(post.Title.Length),
                        Name = newFileName,
                        FileType = fileExtension,
                        CreatedOn = DateTime.Now,
                        ownerId = owner.Id,
                        Title = post.Title,
                        CourseCode = post.CourseCode,
                        Category = post.Category,
                        Lecturer = post.Lecturer,
                        Likes = post.Likes
                    };

                    using (var target = new MemoryStream())
                    {
                        formFile.CopyTo(target);
                        objformFile.DataFiles = target.ToArray();
                    }


                    _context.Files.Add(objformFile);
                    _context.SaveChanges();

                }
            }
            return Redirect("Create");
        }

        public IActionResult Delete(string id)
        {
            // alert gerek
            var post = (from s in _context.Files where s.DocumentId.Equals(id)
                        select s).FirstOrDefault<Files>();
            _context.Files.Remove(post);
            _context.SaveChanges();
            return RedirectToAction("MyCourses");
        }

        public IActionResult LikeAct(string id)
        {

            var post = (from s in _context.Files
                        where s.DocumentId.Equals(id) select s).FirstOrDefault<Files>();
            var currStd = HttpContext.Session.GetObject<Student>("student");
            var like = (from l in _context.Likes
                         where l.DocumentId.Equals(id) && 
                         l.StudentId.Equals(currStd.Id) select l).FirstOrDefault<Likes>();
            
            if (like != null)
            {
                _context.Likes.Remove(like);
                post.Likes -= 1;
                _context.Files.Update(post);
            }
            else
            {
                var objLike = new Likes()
                {
                    DocumentId = id,
                    StudentId = currStd.Id
                };
                _context.Likes.Add(objLike);
                post.Likes += 1;
                _context.Files.Update(post);
            }
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public List<Files> Get_Favorite_Posts()
        {
            List<Files> postList = (from p in _context.Files orderby p.Likes descending select p).Take(5).ToList();
            return postList;
        }
    }
}
