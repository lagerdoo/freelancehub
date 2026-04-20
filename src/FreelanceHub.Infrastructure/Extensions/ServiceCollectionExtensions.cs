using FreelanceHub.Application.Contracts;
using FreelanceHub.Infrastructure.Data;
using FreelanceHub.Infrastructure.Options;
using FreelanceHub.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace FreelanceHub.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));
        services.Configure<UploadOptions>(configuration.GetSection(UploadOptions.SectionName));
        services.Configure<CvImportOptions>(configuration.GetSection(CvImportOptions.SectionName));
        services.Configure<SmtpOptions>(configuration.GetSection(SmtpOptions.SectionName));

        services.AddDbContext<FreelanceHubDbContext>((serviceProvider, options) =>
        {
            var databaseOptions = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
            var hostEnvironment = serviceProvider.GetRequiredService<IHostEnvironment>();
            var provider = databaseOptions.Provider?.Trim().ToLowerInvariant() ?? "sqlite";
            var connectionString = ResolveConnectionString(configuration, databaseOptions, hostEnvironment);

            if (provider is "postgres" or "postgresql" or "npgsql")
            {
                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                });
            }
            else
            {
                options.UseSqlite(connectionString);
            }
        });

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IAdminAuthService, AdminAuthService>();
        services.AddScoped<IAdminContentService, AdminContentService>();
        services.AddScoped<ICvImportService, CvImportService>();
        services.AddScoped<IContentQueryService, ContentQueryService>();
        services.AddScoped<IMediaService, MediaService>();
        services.AddScoped<EmailDeliveryService>();
        services.AddScoped<DatabaseSeeder>();

        return services;
    }

    private static string ResolveConnectionString(IConfiguration configuration, DatabaseOptions databaseOptions, IHostEnvironment hostEnvironment)
    {
        if (!string.IsNullOrWhiteSpace(databaseOptions.ConnectionString))
        {
            return NormalizeSqliteConnectionString(databaseOptions.ConnectionString, databaseOptions.Provider, hostEnvironment);
        }

        var provider = databaseOptions.Provider?.Trim().ToLowerInvariant() ?? "sqlite";
        var name = provider is "postgres" or "postgresql" or "npgsql" ? "PostgreSql" : "Sqlite";
        var configured = configuration.GetConnectionString(name);

        if (provider is "postgres" or "postgresql" or "npgsql")
        {
            if (string.IsNullOrWhiteSpace(configured))
            {
                throw new InvalidOperationException("The PostgreSQL provider is enabled but ConnectionStrings:PostgreSql is not configured.");
            }

            return configured;
        }

        configured ??= "Data Source=App_Data/freelancehub.db";
        return NormalizeSqliteConnectionString(configured, provider, hostEnvironment);
    }

    private static string NormalizeSqliteConnectionString(string connectionString, string? provider, IHostEnvironment hostEnvironment)
    {
        var normalizedProvider = provider?.Trim().ToLowerInvariant() ?? "sqlite";
        if (normalizedProvider is "postgres" or "postgresql" or "npgsql")
        {
            return connectionString;
        }

        const string prefix = "Data Source=";
        if (!connectionString.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            return connectionString;
        }

        var dataSource = connectionString[prefix.Length..].Trim();
        if (Path.IsPathRooted(dataSource))
        {
            return connectionString;
        }

        var absolutePath = Path.Combine(hostEnvironment.ContentRootPath, dataSource.Replace('/', Path.DirectorySeparatorChar));
        var directory = Path.GetDirectoryName(absolutePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        return $"{prefix}{absolutePath}";
    }
}
