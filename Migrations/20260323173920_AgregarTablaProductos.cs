using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backen.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTablaProductos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Stock",
                table: "Productos",
                newName: "UsuarioId");

            migrationBuilder.AddColumn<string>(
                name: "ImagenUrl",
                table: "Productos",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagenUrl",
                table: "Productos");

            migrationBuilder.RenameColumn(
                name: "UsuarioId",
                table: "Productos",
                newName: "Stock");
        }
    }
}
