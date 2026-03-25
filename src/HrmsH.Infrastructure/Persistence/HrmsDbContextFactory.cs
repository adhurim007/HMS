using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HrmsH.Infrastructure.Persistence;

public sealed class HrmsDbContextFactory : IDesignTimeDbContextFactory<HrmsDbContext>
{
    public HrmsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<HrmsDbContext>();
        optionsBuilder.UseSqlServer(
            "Server=ASULEJMANI\\SQLEXPRESS;Database=HRMSH;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true");

        return new HrmsDbContext(optionsBuilder.Options);
    }
}

