using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EligibilityApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CountryQuotas",
                columns: table => new
                {
                    CountryCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    QuotaSdrMillions = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountryQuotas", x => x.CountryCode);
                });

            migrationBuilder.InsertData(
                table: "CountryQuotas",
                columns: new[] { "CountryCode", "QuotaSdrMillions" },
                values: new object[,]
                {
                    { "ARG", 3187.3m },
                    { "EGY", 2037.1m },
                    { "GHA", 738.0m },
                    { "KEN", 542.8m },
                    { "LKA", 578.8m },
                    { "PAK", 2031.0m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CountryQuotas");
        }
    }
}
