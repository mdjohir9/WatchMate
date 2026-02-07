using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatchMate_API.Migrations
{
    /// <inheritdoc />
    public partial class withdraw_rules_add_modification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "WithdrawRule",
                newName: "WithdrawRule",
                newSchema: "dbo");

            migrationBuilder.AlterColumn<string>(
                name: "RuleDescription",
                schema: "dbo",
                table: "WithdrawRule",
                type: "nvarchar(500)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentMethodID",
                schema: "dbo",
                table: "WithdrawRule",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "WithdrawRule",
                schema: "dbo",
                newName: "WithdrawRule");

            migrationBuilder.AlterColumn<string>(
                name: "RuleDescription",
                table: "WithdrawRule",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PaymentMethodID",
                table: "WithdrawRule",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
