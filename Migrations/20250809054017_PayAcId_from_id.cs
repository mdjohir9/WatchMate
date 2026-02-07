using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatchMate_API.Migrations
{
    /// <inheritdoc />
    public partial class PayAcId_from_id : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerPackage_PaymentMethod_PayMethodID",
                table: "CustomerPackage");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "PaymentAccount",
                newName: "PayAcId");

            migrationBuilder.RenameColumn(
                name: "PayMethodID",
                table: "CustomerPackage",
                newName: "PayAcId");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerPackage_PayMethodID",
                table: "CustomerPackage",
                newName: "IX_CustomerPackage_PayAcId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerPackage_PaymentAccount_PayAcId",
                table: "CustomerPackage",
                column: "PayAcId",
                principalTable: "PaymentAccount",
                principalColumn: "PayAcId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerPackage_PaymentAccount_PayAcId",
                table: "CustomerPackage");

            migrationBuilder.RenameColumn(
                name: "PayAcId",
                table: "PaymentAccount",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "PayAcId",
                table: "CustomerPackage",
                newName: "PayMethodID");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerPackage_PayAcId",
                table: "CustomerPackage",
                newName: "IX_CustomerPackage_PayMethodID");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerPackage_PaymentMethod_PayMethodID",
                table: "CustomerPackage",
                column: "PayMethodID",
                principalTable: "PaymentMethod",
                principalColumn: "PayMethodID");
        }
    }
}
