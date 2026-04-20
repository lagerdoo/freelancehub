using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FreelanceHub.Infrastructure.Data;

public sealed class FreelanceHubDesignTimeDbContextFactory : IDesignTimeDbContextFactory<FreelanceHubDbContext>
{
    public FreelanceHubDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<FreelanceHubDbContext>();
        var databasePath = Path.Combine(AppContext.BaseDirectory, "freelancehub-design.db");
        builder.UseSqlite($"Data Source={databasePath}");
        return new FreelanceHubDbContext(builder.Options);
    }
}
