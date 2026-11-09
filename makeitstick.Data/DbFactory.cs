using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace makeitstick.Data
{
    // Centrale plek om een AppDbContext te maken; leest de connection string uit appsettings.json.
    public static class DbFactory
    {
        // Vaste versie i.p.v. ServerVersion.AutoDetect, zodat migrations maken geen draaiende database vereist.
        private static readonly MySqlServerVersion ServerVersion = new(new Version(8, 0, 36));

        public static AppDbContext Create()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseMySql(GetConnectionString(), ServerVersion)
                .UseSeeding((context, _) => DbSeeder.Seed((AppDbContext)context))
                .UseAsyncSeeding(async (context, _, cancellationToken) =>
                    await DbSeeder.SeedAsync((AppDbContext)context, cancellationToken))
                .Options;

            return new AppDbContext(options);
        }

        private static string GetConnectionString()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Local.json", optional: true)
                .Build();

            return configuration.GetConnectionString("Default")
                ?? throw new InvalidOperationException("ConnectionStrings:Default ontbreekt in appsettings.json.");
        }
    }

    // Wordt gebruikt door 'dotnet ef' (migrations add / database update).
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args) => DbFactory.Create();
    }
}
