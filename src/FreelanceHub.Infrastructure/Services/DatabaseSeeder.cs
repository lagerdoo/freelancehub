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
                SiteName = "FreelanceHub Demo",
                TaglineFr = "Template bilingue pour consultant technique et developpeur freelance",
                TaglineEn = "Bilingual template for freelance developers and technical consultants",
                HeroTitleFr = "Un site vitrine premium, simple a adapter a votre parcours",
                HeroTitleEn = "A premium portfolio site designed to adapt to your own profile",
                HeroSubtitleFr = "Ce projet sert de base reutilisable pour presenter services, experience, projets et informations de contact avec une administration securisee.",
                HeroSubtitleEn = "This project is a reusable starting point for showcasing services, experience, projects, and contact details with a secure admin area.",
                AboutSummaryFr = "FreelanceHub Demo est un exemple de site pour independants techniques. Il peut etre personnalise avec votre resume, vos experiences, vos offres et vos liens professionnels sans repartir de zero.",
                AboutSummaryEn = "FreelanceHub Demo is a starter website for technical freelancers. It can be personalized with your resume, experience, offers, and professional links without rebuilding everything from scratch.",
                SkillsFr = ".NET, ASP.NET Core, PostgreSQL, APIs, Architecture logicielle, Modernisation applicative",
                SkillsEn = ".NET, ASP.NET Core, PostgreSQL, APIs, Software architecture, Application modernization",
                ContactEmail = "contact@example.com",
                Location = "Remote / Europe",
                LinkedInUrl = "https://www.linkedin.com/",
                GitHubUrl = "https://github.com/",
                MaltUrl = "https://www.malt.fr/",
                CalendlyUrl = "https://calendly.com/",
                FooterTextFr = "Template de demarrage pour portfolio freelance et conseil technique.",
                FooterTextEn = "Starter template for freelance portfolio and technical consulting websites.",
                FaviconPath = "/favicon.ico",
                MetaTitleFr = "FreelanceHub Demo | Template consultant .NET",
                MetaTitleEn = "FreelanceHub Demo | Freelance .NET consultant template",
                MetaDescriptionFr = "Exemple reutilisable de site portfolio pour developpeur freelance, consultant backend, architecte logiciel ou expert technique.",
                MetaDescriptionEn = "Reusable portfolio website example for freelance developers, backend consultants, software architects, and technical specialists."
            });
        }

        if (seedDemoContent && !await dbContext.Services.AnyAsync(cancellationToken))
        {
            dbContext.Services.AddRange(
                new Service
                {
                    TitleFr = "Architecture backend et API",
                    TitleEn = "Backend and API architecture",
                    SummaryFr = "Conception de plateformes maintenables avec .NET, PostgreSQL et des integrations bien delimitees.",
                    SummaryEn = "Design of maintainable platforms built with .NET, PostgreSQL, and well-defined integrations.",
                    DescriptionFr = "Service type pour independants techniques intervenant sur la structure applicative, les APIs, la couche data et la lisibilite du code.",
                    DescriptionEn = "Template service for technical freelancers who work on application structure, APIs, data layers, and code readability.",
                    IconClass = "bi bi-diagram-3",
                    DisplayOrder = 1,
                    IsPublished = true,
                    Slug = "backend-api-architecture"
                },
                new Service
                {
                    TitleFr = "Modernisation et remise a plat",
                    TitleEn = "Modernization and recovery",
                    SummaryFr = "Reprise d'applications existantes avec reduction de dette technique et feuille de route pragmatique.",
                    SummaryEn = "Recovery of existing applications with technical debt reduction and a pragmatic modernization roadmap.",
                    DescriptionFr = "Exemple de prestation pour rebatir des fondations solides sans bloquer l'activite ni repartir de zero.",
                    DescriptionEn = "Example offer focused on rebuilding solid foundations without freezing delivery or rewriting everything at once.",
                    IconClass = "bi bi-arrow-repeat",
                    DisplayOrder = 2,
                    IsPublished = true,
                    Slug = "application-modernization"
                },
                new Service
                {
                    TitleFr = "Audit fiabilite et delivery",
                    TitleEn = "Reliability and delivery review",
                    SummaryFr = "Analyse des zones de risque sur la production, les deploiements et les traitements critiques.",
                    SummaryEn = "Assessment of production risk, deployment fragility, and critical processing paths.",
                    DescriptionFr = "Exemple d'offre orientee diagnostic, priorisation et plan d'action pour retrouver une execution plus sereine.",
                    DescriptionEn = "Example offer centered on diagnostics, prioritization, and an action plan for steadier delivery.",
                    IconClass = "bi bi-speedometer2",
                    DisplayOrder = 3,
                    IsPublished = true,
                    Slug = "reliability-delivery-review"
                });
        }

        if (seedDemoContent && !await dbContext.Projects.AnyAsync(cancellationToken))
        {
            dbContext.Projects.AddRange(
                new Project
                {
                    TitleFr = "Plateforme de gestion partenaires",
                    TitleEn = "Partner operations platform",
                    SummaryFr = "Exemple de refonte d'un outil metier en plateforme ASP.NET Core avec workflows, reporting et administration.",
                    SummaryEn = "Example redesign of a business tool into an ASP.NET Core platform with workflows, reporting, and administration.",
                    DescriptionFr = "Projet de demonstration pour illustrer l'organisation d'une plateforme metier modulaire avec une base PostgreSQL et une interface admin claire.",
                    DescriptionEn = "Demo project illustrating how to structure a modular business platform with PostgreSQL and a clear administration experience.",
                    TechStack = ".NET 8, ASP.NET Core MVC, PostgreSQL, EF Core, Redis, Docker",
                    ExternalLink = "https://example.com",
                    IsFeatured = true,
                    DisplayOrder = 1,
                    IsPublished = true,
                    Slug = "partner-operations-platform"
                },
                new Project
                {
                    TitleFr = "Portail interne de supervision",
                    TitleEn = "Internal supervision portal",
                    SummaryFr = "Exemple de back-office pour piloter incidents, file d'attente, operations sensibles et suivi d'activite.",
                    SummaryEn = "Example back-office portal for incident handling, queue monitoring, sensitive operations, and activity tracking.",
                    DescriptionFr = "Projet type pour mettre en avant une interface admin sobre, des listes exploitables et une logique metier orientee operations.",
                    DescriptionEn = "Template project showcasing a restrained admin interface, operational lists, and service logic designed for internal teams.",
                    TechStack = ".NET 8, Razor Views, Bootstrap 5, SQLite, EF Core",
                    IsFeatured = true,
                    DisplayOrder = 2,
                    IsPublished = true,
                    Slug = "internal-supervision-portal"
                },
                new Project
                {
                    TitleFr = "Moteur de synchronisation multi-tenant",
                    TitleEn = "Multi-tenant synchronization engine",
                    SummaryFr = "Exemple d'API d'integration pour synchroniser catalogues, commandes ou donnees de reference entre plusieurs systemes.",
                    SummaryEn = "Example integration API used to synchronize catalogs, orders, or reference data across multiple systems.",
                    DescriptionFr = "Projet de demonstration autour de la resilience, de l'idempotence, de la tracabilite et de la reduction des interventions manuelles.",
                    DescriptionEn = "Demo project centered on resilience, idempotency, traceability, and the reduction of manual intervention.",
                    TechStack = ".NET 8, Minimal APIs, PostgreSQL, Hangfire, OpenTelemetry",
                    IsFeatured = true,
                    DisplayOrder = 3,
                    IsPublished = true,
                    Slug = "multi-tenant-synchronization-engine"
                });
        }

        if (seedDemoContent && !await dbContext.ExperienceEntries.AnyAsync(cancellationToken))
        {
            dbContext.ExperienceEntries.AddRange(
                new ExperienceEntry
                {
                    RoleFr = "Consultant technique freelance",
                    RoleEn = "Freelance technical consultant",
                    Company = "Independent practice",
                    Location = "Remote / Europe",
                    StartDate = new DateOnly(2022, 1, 1),
                    DescriptionFr = "Exemple d'activite independante centree sur architecture applicative, accompagnement delivery et modernisation progressive.",
                    DescriptionEn = "Example independent practice focused on application architecture, delivery support, and incremental modernization.",
                    Technologies = ".NET, ASP.NET Core, PostgreSQL, SQL, Docker, Azure",
                    DisplayOrder = 1,
                    IsVisible = true
                },
                new ExperienceEntry
                {
                    RoleFr = "Lead developpeur backend",
                    RoleEn = "Lead backend developer",
                    Company = "SaaS product company",
                    Location = "Remote",
                    StartDate = new DateOnly(2019, 2, 1),
                    EndDate = new DateOnly(2021, 12, 31),
                    DescriptionFr = "Exemple d'experience sur le pilotage technique, la priorisation des travaux structurants et la fiabilisation de flux critiques.",
                    DescriptionEn = "Example experience in technical leadership, structural prioritization, and stabilization of critical business flows.",
                    Technologies = "ASP.NET Core, SQL Server, RabbitMQ, Redis, CI/CD",
                    DisplayOrder = 2,
                    IsVisible = true
                },
                new ExperienceEntry
                {
                    RoleFr = "Ingenieur logiciel",
                    RoleEn = "Software engineer",
                    Company = "Digital delivery studio",
                    Location = "Hybrid",
                    StartDate = new DateOnly(2015, 9, 1),
                    EndDate = new DateOnly(2019, 1, 31),
                    DescriptionFr = "Exemple d'interventions sur des plateformes metier, des APIs et des outils internes pour plusieurs contextes clients.",
                    DescriptionEn = "Example work across business platforms, APIs, and internal tools for several client contexts.",
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
