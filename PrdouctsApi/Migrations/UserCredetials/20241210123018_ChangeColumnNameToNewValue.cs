using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductInventoryManagerAPI.Migrations.UserCredetials
{
    /// <inheritdoc />
    public partial class ChangeColumnNameToNewValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Roles",
                table: "tbl_UserCredetilas",
                newName: "Role");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Role",
                table: "tbl_UserCredetilas",
                newName: "Roles");
        }
    }
}
