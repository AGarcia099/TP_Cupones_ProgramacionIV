using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClientesApi.Migrations
{
    /// <inheritdoc />
    public partial class ClientesApi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    CodCliente = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre_Cliente = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Apellido_Cliente = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.CodCliente);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Clientes");
        }
    }
}
