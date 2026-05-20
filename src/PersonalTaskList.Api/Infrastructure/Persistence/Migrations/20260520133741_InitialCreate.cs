using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalTaskList.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tasks",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    title = table.Column<string>(type: "TEXT", nullable: false),
                    description = table.Column<string>(type: "TEXT", nullable: true),
                    isCompleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    createdAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    updatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    completedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tasks", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tasks");
        }
    }
}
