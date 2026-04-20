using FreelanceHub.Domain.Entities;
using FreelanceHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FreelanceHub.Infrastructure.Services;

public sealed class DatabaseSeeder(
    FreelanceHubDbContext dbContext,
    IConfiguration configuration,
    IHostEnvironment hostEnvironment,
    IPasswordHasher passwordHasher,
    ILogger<DatabaseSeeder> logger)
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.Database.MigrateAsync(cancellationToken);
        var seedDemoContent = configuration.GetValue<bool?>("Seeding:EnableDemoContent") ?? hostEnvironment.IsDevelopment();

        if (seedDemoContent && !await dbContext.SiteSettings.AnyAsync(cancellationToken))
        {
            dbContext.SiteSettings.Add(new SiteSetting
            {
                SiteName = "Marc Karim",
                TaglineFr = "Consultant technique .NET et architecte backend freelance",
                TaglineEn = "Freelance .NET technical consultant and backend architect",
                HeroTitleFr = "Des plateformes .NET robustes, lisibles et pretes pour la production",
                HeroTitleEn = "Robust, readable, production-ready .NET platforms",
                HeroSubtitleFr = "J'interviens sur l'architecture, la modernisation, la performance et la fiabilite des applications critiques.",
                HeroSubtitleEn = "I work on architecture, modernization, performance, and reliability for business-critical applications.",
                ContactEmail = "hello@marckarim.dev",
                Location = "Paris, France",
                LinkedInUrl = "https://www.linkedin.com/",
                GitHubUrl = "https://github.com/",
                MaltUrl = "https://www.malt.fr/",
                CalendlyUrl = "https://calendly.com/",
                FooterTextFr = "Disponible pour des missions d'architecture .NET, audit technique et delivery backend.",
                FooterTextEn = "Available for .NET architecture, technical audit, and backend delivery engagements.",
                FaviconPath = "/favicon.ico",
                MetaTitleFr = "Marc Karim | Consultant .NET freelance",
                MetaTitleEn = "Marc Karim | Freelance .NET consultant",
                MetaDescriptionFr = "Consultant freelance specialise en backend .NET, architecture logicielle, PostgreSQL, APIs et modernisation d'applications.",
                MetaDescriptionEn = "Freelance consultant specialized in .NET backend, software architecture, PostgreSQL, APIs, and application modernization."
            });
        }

        if (seedDemoContent && !await dbContext.Services.AnyAsync(cancellationToken))
        {
            dbContext.Services.AddRange(
                new Service
                {
                    TitleFr = "Architecture backend .NET",
                    TitleEn = "Backend .NET architecture",
                    SummaryFr = "Conception de solutions maintenables, modulaires et sobres en cout d'exploitation.",
                    SummaryEn = "Design of maintainable, modular solutions with disciplined operating costs.",
                    DescriptionFr = "Je structure des applications ASP.NET Core, APIs metier, couches d'acces aux donnees et pipelines d'execution pour tenir dans la duree.",
                    DescriptionEn = "I structure ASP.NET Core applications, business APIs, data access layers, and execution pipelines built to last.",
                    IconClass = "bi bi-diagram-3",
                    DisplayOrder = 1,
                    IsPublished = true,
                    Slug = "backend-dotnet-architecture"
                },
                new Service
                {
                    TitleFr = "Modernisation d'applications",
                    TitleEn = "Application modernization",
                    SummaryFr = "Migration progressive d'heritage technique vers une base plus saine et plus simple a faire evoluer.",
                    SummaryEn = "Progressive migration from legacy constraints to a healthier and easier-to-evolve codebase.",
                    DescriptionFr = "Je reprends des applications existantes, reduis la dette technique, separe les responsabilites et prepare un delivery plus fiable.",
                    DescriptionEn = "I take over existing applications, reduce technical debt, separate responsibilities, and prepare a more reliable delivery model.",
                    IconClass = "bi bi-arrow-repeat",
                    DisplayOrder = 2,
                    IsPublished = true,
                    Slug = "application-modernization"
                },
                new Service
                {
                    TitleFr = "Audit performance et fiabilite",
                    TitleEn = "Performance and reliability audit",
                    SummaryFr = "Analyse des points de friction sur les APIs, la base de donnees et les traitements critiques.",
                    SummaryEn = "Analysis of bottlenecks across APIs, databases, and critical processing flows.",
                    DescriptionFr = "Je cible les goulets d'etranglement, les risques de production et les leviers de simplification pour retrouver de la marge.",
                    DescriptionEn = "I target bottlenecks, production risks, and simplification opportunities to regain execution margin.",
                    IconClass = "bi bi-speedometer2",
                    DisplayOrder = 3,
                    IsPublished = true,
                    Slug = "performance-reliability-audit"
                });
        }

        if (seedDemoContent && !await dbContext.Projects.AnyAsync(cancellationToken))
        {
            dbContext.Projects.AddRange(
                new Project
                {
                    TitleFr = "Plateforme B2B de gestion de commandes",
                    TitleEn = "B2B order management platform",
                    SummaryFr = "Refonte d'un socle monolithique en plateforme modulaire ASP.NET Core et PostgreSQL.",
                    SummaryEn = "Transformation of a brittle monolith into a modular ASP.NET Core and PostgreSQL platform.",
                    DescriptionFr = "Mission orientee architecture et delivery avec rationalisation des traitements, securisation des integrations et baisse des incidents.",
                    DescriptionEn = "Architecture and delivery engagement focused on streamlining workflows, securing integrations, and reducing incidents.",
                    TechStack = ".NET 8, ASP.NET Core MVC, PostgreSQL, EF Core, Redis, Docker",
                    ExternalLink = "https://example.com",
                    IsFeatured = true,
                    DisplayOrder = 1,
                    IsPublished = true,
                    Slug = "b2b-order-management-platform"
                },
                new Project
                {
                    TitleFr = "Back office de supervision metier",
                    TitleEn = "Business supervision back office",
                    SummaryFr = "Conception d'un portail interne pour piloter flux, incidents et operations sensibles.",
                    SummaryEn = "Design of an internal portal to oversee workflows, incidents, and sensitive operations.",
                    DescriptionFr = "Implementation d'une interface admin claire, d'une couche service testable et d'un modele de donnees oriente exploitation.",
                    DescriptionEn = "Implementation of a clear admin interface, a testable service layer, and an operations-oriented data model.",
                    TechStack = ".NET 8, Razor Views, Bootstrap 5, SQLite, EF Core",
                    IsFeatured = true,
                    DisplayOrder = 2,
                    IsPublished = true,
                    Slug = "business-supervision-back-office"
                },
                new Project
                {
                    TitleFr = "API de synchronisation multi-systemes",
                    TitleEn = "Multi-system synchronization API",
                    SummaryFr = "Stabilisation d'une API d'integration confrontee a des volumes et a des contrats externes heterogenes.",
                    SummaryEn = "Stabilization of an integration API facing volume pressure and heterogeneous external contracts.",
                    DescriptionFr = "Travail sur la resilience, l'idempotence, la tracabilite et la reduction des reprises manuelles.",
                    DescriptionEn = "Focused work on resilience, idempotency, traceability, and the reduction of manual recovery tasks.",
                    TechStack = ".NET 8, Minimal APIs, PostgreSQL, Hangfire, OpenTelemetry",
                    IsFeatured = true,
                    DisplayOrder = 3,
                    IsPublished = true,
                    Slug = "multi-system-synchronization-api"
                });
        }

        if (seedDemoContent && !await dbContext.ExperienceEntries.AnyAsync(cancellationToken))
        {
            dbContext.ExperienceEntries.AddRange(
                new ExperienceEntry
                {
                    RoleFr = "Consultant backend .NET freelance",
                    RoleEn = "Freelance .NET backend consultant",
                    Company = "Independent",
                    Location = "Paris / Remote",
                    StartDate = new DateOnly(2022, 1, 1),
                    DescriptionFr = "Interventions sur architecture, revue technique, delivery backend et reprise d'applications sensibles.",
                    DescriptionEn = "Engagements across architecture, technical review, backend delivery, and recovery of sensitive applications.",
                    Technologies = ".NET, ASP.NET Core, PostgreSQL, SQL, Docker, Azure",
                    DisplayOrder = 1,
                    IsVisible = true
                },
                new ExperienceEntry
                {
                    RoleFr = "Lead developpeur .NET",
                    RoleEn = "Lead .NET developer",
                    Company = "Fintech Scale-up",
                    Location = "Paris",
                    StartDate = new DateOnly(2019, 2, 1),
                    EndDate = new DateOnly(2021, 12, 31),
                    DescriptionFr = "Pilotage technique d'une equipe backend, refonte de flux critiques et structuration du delivery.",
                    DescriptionEn = "Technical leadership for a backend team, redesign of critical flows, and delivery process hardening.",
                    Technologies = "ASP.NET Core, SQL Server, RabbitMQ, Redis, CI/CD",
                    DisplayOrder = 2,
                    IsVisible = true
                },
                new ExperienceEntry
                {
                    RoleFr = "Ingenieur logiciel .NET",
                    RoleEn = ".NET software engineer",
                    Company = "Digital Services Company",
                    Location = "Lyon",
                    StartDate = new DateOnly(2015, 9, 1),
                    EndDate = new DateOnly(2019, 1, 31),
                    DescriptionFr = "Developpement de plateformes metier, APIs et outils internes pour des clients grands comptes.",
                    DescriptionEn = "Development of business platforms, APIs, and internal tools for enterprise clients.",
                    Technologies = "C#, ASP.NET MVC, Web API, Entity Framework, SQL",
                    DisplayOrder = 3,
                    IsVisible = true
                });
        }

        if (!await dbContext.AdminUsers.AnyAsync(cancellationToken))
        {
            var password = configuration["SeedAdmin:Password"];

            if (string.IsNullOrWhiteSpace(password) && hostEnvironment.IsDevelopment())
            {
                password = "DevAdmin#2026";
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                logger.LogWarning("Seed admin user was skipped because SeedAdmin:Password is not configured.");
            }
            else
            {
                dbContext.AdminUsers.Add(new AdminUser
                {
                    Username = configuration["SeedAdmin:Username"] ?? "admin",
                    Email = configuration["SeedAdmin:Email"] ?? "admin@localhost",
                    DisplayName = configuration["SeedAdmin:DisplayName"] ?? "Site Administrator",
                    PasswordHash = passwordHasher.Hash(password),
                    IsActive = true
                });
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
