using AppDating.API.Model.Domain;
using Microsoft.EntityFrameworkCore;

namespace AppDating.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<AppUser> AppUsers { get; set; }
    }
}
