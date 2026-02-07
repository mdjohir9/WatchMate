using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatchMate_API.Migrations
{
    /// <inheritdoc />
    public partial class withdraw_rules_table_add : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RuleId",
                table: "Withdraw",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiryDate",
                table: "CustomerPackage",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.CreateTable(
                name: "WithdrawRule",
                columns: table => new
                {
                    RuleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentMethodID = table.Column<int>(type: "int", nullable: false),
                    RuleTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RuleDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MinAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DailyLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FeePercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WithdrawRule", x => x.RuleId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Withdraw_RuleId",
                table: "Withdraw",
                column: "RuleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Withdraw_WithdrawRule_RuleId",
                table: "Withdraw",
                column: "RuleId",
                principalTable: "WithdrawRule",
                principalColumn: "RuleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Withdraw_WithdrawRule_RuleId",
                table: "Withdraw");

            migrationBuilder.DropTable(
                name: "WithdrawRule");

            migrationBuilder.DropIndex(
                name: "IX_Withdraw_RuleId",
                table: "Withdraw");

            migrationBuilder.DropColumn(
                name: "RuleId",
                table: "Withdraw");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiryDate",
                table: "CustomerPackage",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
