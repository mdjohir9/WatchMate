using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatchMate_API.Migrations
{
    /// <inheritdoc />
    public partial class customer_package_add_payment_method : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PayMethodID",
                table: "CustomerPackage",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PaymentMethod",
                columns: table => new
                {
                    PayMethodID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethod", x => x.PayMethodID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPackage_PayMethodID",
                table: "CustomerPackage",
                column: "PayMethodID");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerPackage_PaymentMethod_PayMethodID",
                table: "CustomerPackage",
                column: "PayMethodID",
                principalTable: "PaymentMethod",
                principalColumn: "PayMethodID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerPackage_PaymentMethod_PayMethodID",
                table: "CustomerPackage");

            migrationBuilder.DropTable(
                name: "PaymentMethod");

            migrationBuilder.DropIndex(
                name: "IX_CustomerPackage_PayMethodID",
                table: "CustomerPackage");

            migrationBuilder.DropColumn(
                name: "PayMethodID",
                table: "CustomerPackage");
        }
    }
}
