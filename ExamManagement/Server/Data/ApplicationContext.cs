using ExamManagement.Server.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExamManagement.Server.Data
{
    public class ApplicationContext:IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationContext(DbContextOptions options) : base(options) { }

        public DbSet<Course> Course { get; set; }
        public DbSet<Faculty> Faculty { get; set; }
        public DbSet<UserDetail> UserDetail { get; set; }
        public DbSet<UserDetailExtension> UserDetailExtension { get; set; }
        public DbSet<UserDetailExtensionAbove> UserDetailExtensionAbove { get; set; }
    }
}
