using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatchMate_API.Migrations
{
    /// <inheritdoc />
    public partial class refaralrewardunerthe_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.RenameTable(
                name: "ReferralReward",
                newName: "ReferralReward",
                newSchema: "dbo");

            migrationBuilder.AlterColumn<string>(
                name: "ReferralCode",
                table: "CustomerInfo",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInfo_ReferralCode",
                table: "CustomerInfo",
                column: "ReferralCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CustomerInfo_ReferralCode",
                table: "CustomerInfo");

            migrationBuilder.RenameTable(
                name: "ReferralReward",
                schema: "dbo",
                newName: "ReferralReward");

            migrationBuilder.AlterColumn<string>(
                name: "ReferralCode",
                table: "CustomerInfo",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
