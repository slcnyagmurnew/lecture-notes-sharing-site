using Moon.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Moon_.Models
{
    public class JsonDataHelper
    {
        static Dictionary<string, string> CourseDictionary = new Dictionary<string, string>();

        public static void CreateDict()
        {
            string path = Directory.GetCurrentDirectory();
            var json = System.IO.File.ReadAllText(path + "\\wwwroot\\jsondata\\data.json");
            var myobj = JArray.Parse(json);
            var result = JsonConvert.DeserializeObject<List<Files>>(myobj.ToString());
            foreach (var file in result)
            {
                CourseDictionary.Add(file.CourseCode, file.Category);
            }
        }

        public Dictionary<string, string> GetDict()
        {
            return CourseDictionary;
        }

    }
}
