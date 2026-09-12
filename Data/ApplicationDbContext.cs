using CKM_ManagementSystem.Models.Entities;
using Microsoft.EntityFrameworkCore;


namespace CKM_ManagementSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet <Department> Departments { get; set; }
        public DbSet<Menu> Menus { get; set; }
    }
}
