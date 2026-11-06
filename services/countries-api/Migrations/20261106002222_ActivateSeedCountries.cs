using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace countries_api.Migrations
{
    /// <inheritdoc />
    public partial class ActivateSeedCountries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "IsoCode",
                keyValue: "ARG",
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "IsoCode",
                keyValue: "EGY",
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "IsoCode",
                keyValue: "GHA",
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "IsoCode",
                keyValue: "KEN",
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "IsoCode",
                keyValue: "LKA",
                column: "IsActive",
                value: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "IsoCode",
                keyValue: "ARG",
                column: "IsActive",
                value: false);

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "IsoCode",
                keyValue: "EGY",
                column: "IsActive",
                value: false);

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "IsoCode",
                keyValue: "GHA",
                column: "IsActive",
                value: false);

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "IsoCode",
                keyValue: "KEN",
                column: "IsActive",
                value: false);

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "IsoCode",
                keyValue: "LKA",
                column: "IsActive",
                value: false);
        }
    }
}
