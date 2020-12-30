using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moon.Entities;
using Moon.Models;
using Moon.SessionExtensions;
using Moon_.Models;
using PagedList.Core;

namespace Moon_.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly StudentContext _context;

        public AdminController(StudentContext context)
        {
            _context = context;
        }

        readonly JsonDataHelper _dataHelper = new JsonDataHelper();

        public IActionResult Index(string sortOrder, string currentFilter, string SearchCode, string GroupValue)
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
            if (SearchCode == null)
            {
                SearchCode = currentFilter;
            }

            ViewBag.CourseCode = new SelectList(_dataHelper.GetDict().Keys.ToList());

            return View(posts);
            // table contextinin kullanilabilmesi için yukarıda olusturulan nesne kullanıldı
        }

        [HttpPost]
        public IActionResult Index(IFormCollection form)
        {
            var optionValue = form["CourseCodeDrop"];
            var optionCategory = form["GroupValue"];
            return RedirectToAction("Index", new { SearchCode = optionValue, GroupValue = optionCategory });
        }

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

        public IActionResult Delete(string id)
        {
            var post = (from s in _context.Files
                        where s.DocumentId.Equals(id)
                        select s).FirstOrDefault<Files>();
            _context.Files.Remove(post);
            _context.SaveChanges();
            TempData["success"] = "File deleted successfully!";
            return RedirectToAction("Index");
        }

        public IActionResult DeleteStudent(string id)
        {
            var student = (from s in _context.Students where s.Id.Equals(id) select s).FirstOrDefault<Student>();
            _context.Students.Remove(student);
            _context.SaveChanges();
            TempData["success"] = "User deleted successfully!";
            return RedirectToAction("Students");
        }

        public IActionResult Students()
        {
            var studentlist = (from s in _context.Students select s);
            return View(studentlist);
        }

        [HttpPost]
        public IActionResult Students(string id)
        {
            var student = (from s in _context.Students where s.Id.Equals(id) select s).ToList();
            if(student.Count == 0)
            {
                ViewData["error"] = "User does not exist, please enter a valid username!";
                return View(student);
            }
            return View(student);
        }

        public IActionResult LogOut()
        {
            foreach (var cookie in Request.Cookies.Keys)
            {
                if (cookie == ".AspNetCore.Session" || cookie == ".AspNetCore.Identity.Application")
                    Response.Cookies.Delete(cookie);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
