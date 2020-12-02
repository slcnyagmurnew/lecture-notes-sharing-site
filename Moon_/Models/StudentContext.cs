using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Moon.Entities;
using Moon_.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moon.Models
{
    public class StudentContext:IdentityDbContext<Student>
    {
        public StudentContext(DbContextOptions<StudentContext>options):base(options)
        {

        }

        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<Files>Files { get; set; }
        public virtual DbSet<Likes> Likes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
