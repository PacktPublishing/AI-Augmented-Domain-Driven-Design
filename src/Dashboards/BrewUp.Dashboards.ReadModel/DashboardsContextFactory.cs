using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BrewUp.Dashboards.ReadModel;

public class DashboardsContextFactory : IDesignTimeDbContextFactory<DashboardsContext>
{
    public DashboardsContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DashboardsContext>();
        optionsBuilder.UseSqlServer("");

        return new DashboardsContext(optionsBuilder.Options);
    }
}

