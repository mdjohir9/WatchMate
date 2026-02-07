using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatchMate_API.Migrations
{
    /// <inheritdoc />
    public partial class IsActive_toStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "CustomerPackage");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Package",
                newName: "Status");

            migrationBuilder.AddColumn<byte>(
                name: "Status",
                table: "CustomerPackage",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "CustomerPackage");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Package",
                newName: "IsActive");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "CustomerPackage",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
