# FreelanceHub

Production-ready bilingual freelance website and admin panel built with ASP.NET Core MVC (.NET 8), EF Core, Razor Views, and Bootstrap 5.

## Architecture

- `src/FreelanceHub.Domain`: entities and shared domain base types
- `src/FreelanceHub.Application`: service contracts and application models
- `src/FreelanceHub.Infrastructure`: EF Core, provider switching, authentication support, media storage, seed data
- `src/FreelanceHub.Web`: MVC host, public site, admin area, views, static assets

## Main features

- Public pages: Home, About, Services, Projects, Experience, Contact
- Bilingual public experience with `fr` and `en` routes
- Secure admin area with cookie authentication
- CRUD for services, projects, experience entries
- Contact inbox
- Site settings management
- Media upload and media library
- CV import workflow for admin-assisted profile setup from a text-based PDF resume
- SQLite by default, PostgreSQL-ready configuration
- EF Core migrations and seed data
- Production hardening for cookies, headers, proxy deployment, uploads, and PostgreSQL-first production config

## Generic demo content

The project now ships with reusable demo content instead of identity-specific profile data.

- Site settings use neutral branding: `FreelanceHub Demo`
- Services demonstrate common freelance technical offers
- Projects are realistic but generic showcase examples
- Experience entries describe reusable consultant/developer career patterns
- Contact information and links use neutral placeholders

Demo content is seeded only when the related tables are empty and demo seeding is enabled.
It does not overwrite existing production data.

## Local setup

1. Restore packages:

   ```powershell
   dotnet restore FreelanceHub.slnx
   ```

2. Build:

   ```powershell
   dotnet build FreelanceHub.slnx
   ```

3. Apply the database migration:

   ```powershell
   dotnet tool run dotnet-ef database update --project src/FreelanceHub.Infrastructure/FreelanceHub.Infrastructure.csproj --startup-project src/FreelanceHub.Web/FreelanceHub.Web.csproj
   ```

4. Run the web app:

   ```powershell
   dotnet run --project src/FreelanceHub.Web/FreelanceHub.Web.csproj
   ```

5. Open:

   - Public site: `/fr` or `/en`
   - Admin login: `/admin/account/login`

## Development admin credentials

- Username: `admin`
- Password: `DevAdmin#2026`

This password comes from `appsettings.Development.json`. For production, override `SeedAdmin:Password` with secure configuration and rotate the account after first access.

## Database provider switch

Default provider is SQLite:

```json
"Database": {
  "Provider": "Sqlite"
}
```

To use PostgreSQL, switch provider to `PostgreSql` and update the `PostgreSql` connection string in `src/FreelanceHub.Web/appsettings.json` or environment-specific configuration.

For production, prefer environment variables instead of JSON secrets:

```bash
Database__Provider=PostgreSql
ConnectionStrings__PostgreSql=Host=127.0.0.1;Port=5432;Database=freelancehub;Username=freelancehub_app;Password=REPLACE_ME
```

## Migrations

Create a new migration:

```powershell
dotnet tool run dotnet-ef migrations add <MigrationName> --project src/FreelanceHub.Infrastructure/FreelanceHub.Infrastructure.csproj --startup-project src/FreelanceHub.Web/FreelanceHub.Web.csproj --output-dir Data/Migrations
```

Apply migrations:

```powershell
dotnet tool run dotnet-ef database update --project src/FreelanceHub.Infrastructure/FreelanceHub.Infrastructure.csproj --startup-project src/FreelanceHub.Web/FreelanceHub.Web.csproj
```

## Additional project docs

- QA checklist: [docs/QA_CHECKLIST.md](docs/QA_CHECKLIST.md)
- CV import guide: [docs/CV_IMPORT.md](docs/CV_IMPORT.md)
- OVH Linux VPS deployment: [docs/DEPLOY_OVH_LINUX_VPS.md](docs/DEPLOY_OVH_LINUX_VPS.md)
