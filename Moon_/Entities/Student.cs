using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Moon.Entities
{
    public class Student
    {
        public string name { get; set; }
        public string surname { get; set; }
        public string id { get; set; }
        public string password { get; set; }
        public string department { get; set; }
    }
}
