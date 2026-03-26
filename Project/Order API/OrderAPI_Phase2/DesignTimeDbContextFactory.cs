using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using OrderAPI_Phase2.Data;

namespace OrderAPI_Phase2
{
    public class DesignTimeDbContextFactory
          : IDesignTimeDbContextFactory<OrderDbContext>
    {
        public OrderDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<OrderDbContext>();
            optionsBuilder.UseSqlServer(
                "Server=192.168.4.198;Database=Microservices;User Id=sa;Password=Rlsa*001;TrustServerCertificate=True;Encrypt=False");
            return new OrderDbContext(optionsBuilder.Build());
        }
    }
}
