using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatchMate_API.Migrations
{
    /// <inheritdoc />
    public partial class logo_add_on_payment_method : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Logo",
                table: "PaymentAccount",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Logo",
                table: "PaymentAccount");
        }
    }
}
