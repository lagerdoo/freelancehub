using FreelanceHub.Domain.Common;
using FreelanceHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FreelanceHub.Infrastructure.Data;

public sealed class FreelanceHubDbContext(DbContextOptions<FreelanceHubDbContext> options) : DbContext(options)
{
    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ExperienceEntry> ExperienceEntries => Set<ExperienceEntry>();
    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();
    public DbSet<SiteSetting> SiteSettings => Set<SiteSetting>();
    public DbSet<UploadedMedia> UploadedMedia => Set<UploadedMedia>();

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdminUser>(entity =>
        {
            entity.ToTable("AdminUsers");
            entity.HasIndex(x => x.Username).IsUnique();
            entity.HasIndex(x => x.Email).IsUnique();
            entity.Property(x => x.Username).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(256).IsRequired();
            entity.Property(x => x.DisplayName).HasMaxLength(150).IsRequired();
            entity.Property(x => x.PasswordHash).HasMaxLength(512).IsRequired();
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.ToTable("Services");
            entity.HasIndex(x => x.Slug).IsUnique();
            entity.Property(x => x.TitleFr).HasMaxLength(160).IsRequired();
            entity.Property(x => x.TitleEn).HasMaxLength(160).IsRequired();
            entity.Property(x => x.SummaryFr).HasMaxLength(500).IsRequired();
            entity.Property(x => x.SummaryEn).HasMaxLength(500).IsRequired();
            entity.Property(x => x.IconClass).HasMaxLength(120);
            entity.Property(x => x.ImagePath).HasMaxLength(300);
            entity.Property(x => x.Slug).HasMaxLength(180).IsRequired();
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.ToTable("Projects");
            entity.HasIndex(x => x.Slug).IsUnique();
            entity.Property(x => x.TitleFr).HasMaxLength(160).IsRequired();
            entity.Property(x => x.TitleEn).HasMaxLength(160).IsRequired();
            entity.Property(x => x.SummaryFr).HasMaxLength(500).IsRequired();
            entity.Property(x => x.SummaryEn).HasMaxLength(500).IsRequired();
            entity.Property(x => x.TechStack).HasMaxLength(500).IsRequired();
            entity.Property(x => x.MainImagePath).HasMaxLength(300);
            entity.Property(x => x.ExternalLink).HasMaxLength(300);
            entity.Property(x => x.Slug).HasMaxLength(180).IsRequired();
        });

        modelBuilder.Entity<ExperienceEntry>(entity =>
        {
            entity.ToTable("ExperienceEntries");
            entity.Property(x => x.RoleFr).HasMaxLength(160).IsRequired();
            entity.Property(x => x.RoleEn).HasMaxLength(160).IsRequired();
            entity.Property(x => x.Company).HasMaxLength(160).IsRequired();
            entity.Property(x => x.Location).HasMaxLength(160).IsRequired();
            entity.Property(x => x.Technologies).HasMaxLength(500).IsRequired();
        });

        modelBuilder.Entity<ContactMessage>(entity =>
        {
            entity.ToTable("ContactMessages");
            entity.Property(x => x.Name).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(256).IsRequired();
            entity.Property(x => x.Subject).HasMaxLength(140).IsRequired();
            entity.Property(x => x.Message).HasMaxLength(3000).IsRequired();
            entity.Property(x => x.LastReplySubject).HasMaxLength(180);
            entity.Property(x => x.LastReplyPreview).HasMaxLength(500);
            entity.Property(x => x.LastReplyError).HasMaxLength(500);
        });

        modelBuilder.Entity<SiteSetting>(entity =>
        {
            entity.ToTable("SiteSettings");
            entity.Property(x => x.SiteName).HasMaxLength(150).IsRequired();
            entity.Property(x => x.TaglineFr).HasMaxLength(250).IsRequired();
            entity.Property(x => x.TaglineEn).HasMaxLength(250).IsRequired();
            entity.Property(x => x.ContactEmail).HasMaxLength(256).IsRequired();
            entity.Property(x => x.Location).HasMaxLength(160).IsRequired();
            entity.Property(x => x.LinkedInUrl).HasMaxLength(300);
            entity.Property(x => x.GitHubUrl).HasMaxLength(300);
            entity.Property(x => x.MaltUrl).HasMaxLength(300);
            entity.Property(x => x.CalendlyUrl).HasMaxLength(300);
            entity.Property(x => x.LogoPath).HasMaxLength(300);
            entity.Property(x => x.FaviconPath).HasMaxLength(300);
            entity.Property(x => x.MetaTitleFr).HasMaxLength(180).IsRequired();
            entity.Property(x => x.MetaTitleEn).HasMaxLength(180).IsRequired();
            entity.Property(x => x.MetaDescriptionFr).HasMaxLength(320).IsRequired();
            entity.Property(x => x.MetaDescriptionEn).HasMaxLength(320).IsRequired();
        });

        modelBuilder.Entity<UploadedMedia>(entity =>
        {
            entity.ToTable("UploadedMedia");
            entity.Property(x => x.OriginalFileName).HasMaxLength(260).IsRequired();
            entity.Property(x => x.StoredFileName).HasMaxLength(260).IsRequired();
            entity.Property(x => x.ContentType).HasMaxLength(100).IsRequired();
            entity.Property(x => x.RelativePath).HasMaxLength(300).IsRequired();
            entity.Property(x => x.AltTextFr).HasMaxLength(250);
            entity.Property(x => x.AltTextEn).HasMaxLength(250);
        });

        base.OnModelCreating(modelBuilder);
    }

    private void UpdateAuditFields()
    {
        var utcNow = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<EntityBase>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAtUtc = utcNow;
                entry.Entity.UpdatedAtUtc = utcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAtUtc = utcNow;
            }
        }
    }
}
