using Microsoft.EntityFrameworkCore;
using Moon.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moon.Models
{
    public class StudentContext:DbContext
    {
        public StudentContext(DbContextOptions<StudentContext>options):base(options)
        {

        }

        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<Files>Files { get; set; }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
