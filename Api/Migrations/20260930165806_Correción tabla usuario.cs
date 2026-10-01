using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class Correcióntablausuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Usuarios");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Usuarios",
                newName: "UserName");

            migrationBuilder.RenameIndex(
                name: "IX_Usuarios_Username",
                table: "Usuarios",
                newName: "IX_Usuarios_UserName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "Usuarios",
                newName: "Username");

            migrationBuilder.RenameIndex(
                name: "IX_Usuarios_UserName",
                table: "Usuarios",
                newName: "IX_Usuarios_Username");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Usuarios",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);
        }
    }
}
