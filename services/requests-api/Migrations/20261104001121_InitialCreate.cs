using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RequestsApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FundingRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CountryCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CountryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fund = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EligibilityReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DecidedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FundingRequests", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "FundingRequests",
                columns: new[] { "Id", "Amount", "CountryCode", "CountryName", "CreatedAt", "DecidedAt", "EligibilityReason", "Fund", "Status" },
                values: new object[,]
                {
                    { 1, 900m, "GHA", "Ghana", new DateTime(2026, 11, 3, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "ECF", "Submitted" },
                    { 2, 1000m, "KEN", "Kenya", new DateTime(2026, 11, 3, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "EFF", "Submitted" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FundingRequests");
        }
    }
}
