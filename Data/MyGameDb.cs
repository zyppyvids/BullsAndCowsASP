using Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class MyGameDb : DbContext
    {
        public virtual DbSet<User> Users { get; set; }


        public MyGameDb()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=gamedb.db");
        }
    }
}
