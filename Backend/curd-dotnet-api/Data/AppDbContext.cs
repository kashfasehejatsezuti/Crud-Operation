using Microsoft.EntityFrameworkCore;

namespace curd_dotnet_api.Data
{
    public class AppDbContext : DbContext
    {
        //Constructor
            public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        //Method which help to inisialize DBTable
        public DbSet<Employee> Employees { get; set; }
    }
}
