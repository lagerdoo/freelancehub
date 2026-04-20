using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreelanceHub.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdminMessageRepliesAndSorting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FollowUpStatus",
                table: "ContactMessages",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastReplyAtUtc",
                table: "ContactMessages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastReplyError",
                table: "ContactMessages",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastReplyPreview",
                table: "ContactMessages",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastReplySubject",
                table: "ContactMessages",
                type: "TEXT",
                maxLength: 180,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FollowUpStatus",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "LastReplyAtUtc",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "LastReplyError",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "LastReplyPreview",
                table: "ContactMessages");

            migrationBuilder.DropColumn(
                name: "LastReplySubject",
                table: "ContactMessages");
        }
    }
}
