using Microsoft.EntityFrameworkCore;
using ProductInventoryManagerAPI.Models.Entities;

namespace ProductInventoryManagerAPI.Data
{
    public class UserCredetialsContext(DbContextOptions<UserCredetialsContext> options) : DbContext(options)
    {
        public DbSet<UserCredetials> UserCredetials { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserCredetials>().HasKey(e => e.UserId);
            modelBuilder.Entity<UserCredetials>().Property(e => e.UserId).ValueGeneratedOnAdd().UseIdentityColumn(500000, 1);
            modelBuilder.Entity<UserCredetials>().ToTable("tbl_UserCredetilas");
        }
    }
}
