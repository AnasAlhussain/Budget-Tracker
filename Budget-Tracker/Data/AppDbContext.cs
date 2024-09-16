using Budget_Tracker.Models;
using Microsoft.EntityFrameworkCore;

namespace Budget_Tracker.Data
{
    public class AppDbContext :DbContext
    {
        public AppDbContext(DbContextOptions options) :base(options)
        {
            
        }


        public DbSet<Category>  Categories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }
}
