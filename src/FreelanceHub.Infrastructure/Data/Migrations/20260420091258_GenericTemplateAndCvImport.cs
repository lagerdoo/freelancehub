using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreelanceHub.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class GenericTemplateAndCvImport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AboutSummaryEn",
                table: "SiteSettings",
                type: "TEXT",
                maxLength: 3000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AboutSummaryFr",
                table: "SiteSettings",
                type: "TEXT",
                maxLength: 3000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SkillsEn",
                table: "SiteSettings",
                type: "TEXT",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SkillsFr",
                table: "SiteSettings",
                type: "TEXT",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "CvImportRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OriginalFileName = table.Column<string>(type: "TEXT", maxLength: 260, nullable: false),
                    StoredFileName = table.Column<string>(type: "TEXT", maxLength: 260, nullable: false),
                    ContentType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    FileSize = table.Column<long>(type: "INTEGER", nullable: false),
                    StoragePath = table.Column<string>(type: "TEXT", maxLength: 400, nullable: false),
                    ExtractedText = table.Column<string>(type: "TEXT", nullable: false),
                    AppliedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CvImportRecords", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CvImportRecords");

            migrationBuilder.DropColumn(
                name: "AboutSummaryEn",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "AboutSummaryFr",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "SkillsEn",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "SkillsFr",
                table: "SiteSettings");
        }
    }
}
