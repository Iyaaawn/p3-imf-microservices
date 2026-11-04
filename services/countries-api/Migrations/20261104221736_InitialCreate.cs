using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace countries_api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    IsoCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.IsoCode);
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "IsoCode", "IsActive", "Name" },
                values: new object[,]
                {
                    { "ARG", false, "Argentina" },
                    { "EGY", false, "Egypt" },
                    { "GHA", false, "Ghana" },
                    { "KEN", false, "Kenya" },
                    { "LKA", false, "Sri Lanka" },
                    { "PAK", false, "Pakistan" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
