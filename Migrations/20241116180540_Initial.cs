using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SnaptagOwnKioskInternalBackend.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PurchaseHistories",
                columns: table => new
                {
                    Index = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EventIndex = table.Column<int>(type: "INTEGER", nullable: false),
                    MachineIndex = table.Column<int>(type: "INTEGER", nullable: false),
                    PhotoAuthNumber = table.Column<string>(type: "TEXT", nullable: false),
                    Amount = table.Column<int>(type: "INTEGER", nullable: false),
                    AuthSeqNum = table.Column<string>(type: "TEXT", nullable: false),
                    ApprovalNumber = table.Column<string>(type: "TEXT", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: true),
                    isRefunded = table.Column<bool>(type: "INTEGER", nullable: false),
                    isUploaded = table.Column<bool>(type: "INTEGER", nullable: false),
                    isPrinted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Details = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseHistories", x => x.Index);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchaseHistories");
        }
    }
}
