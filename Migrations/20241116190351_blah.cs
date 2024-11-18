using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SnaptagOwnKioskInternalBackend.Migrations
{
    /// <inheritdoc />
    public partial class blah : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date",
                table: "PurchaseHistories",
                newName: "PurchaseDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PurchaseDate",
                table: "PurchaseHistories",
                newName: "Date");
        }
    }
}
