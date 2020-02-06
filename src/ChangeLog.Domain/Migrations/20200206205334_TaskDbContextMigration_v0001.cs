using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ChangeLog.Domain.Migrations
{
    public partial class TaskDbContextMigration_v0001 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(nullable: false),
                    StartedAt = table.Column<DateTimeOffset>(nullable: true),
                    FinishedAt = table.Column<DateTimeOffset>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 256, nullable: true),
                    CancelledBy = table.Column<string>(maxLength: 256, nullable: true),
                    PercentComplete = table.Column<int>(nullable: false),
                    Payload = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IndexStatus",
                table: "Tasks",
                column: "Status");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tasks");
        }
    }
}
