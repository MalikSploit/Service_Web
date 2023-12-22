using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookService.Migrations;

/// <inheritdoc />
public partial class Init : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Book",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Title = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                Author = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                ImageUrl = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                Price = table.Column<string>(type: "decimal(18,2)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Book", x => x.Id);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable(name: "Book");
}