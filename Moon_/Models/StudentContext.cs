using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Moon.Entities;
using Moon_.Entities;
using System;

namespace Moon.Models
{
    public class StudentContext:IdentityDbContext<IdentityUser, IdentityRole, string>
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
