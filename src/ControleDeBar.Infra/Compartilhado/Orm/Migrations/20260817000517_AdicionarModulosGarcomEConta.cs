using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleDeBar.Infra.Compartilhado.Orm.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarModulosGarcomEConta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Produto",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrecoVenda = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TBGarcom",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBGarcom", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TBConta",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MesaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GarcomId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NomeCliente = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DataAbertura = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Situacao = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBConta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBConta_TBGarcom_GarcomId",
                        column: x => x.GarcomId,
                        principalTable: "TBGarcom",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TBConta_TBMesa_MesaId",
                        column: x => x.MesaId,
                        principalTable: "TBMesa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PedidoConta",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProdutoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    PrecoUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ContaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidoConta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PedidoConta_Produto_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PedidoConta_TBConta_ContaId",
                        column: x => x.ContaId,
                        principalTable: "TBConta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PedidoConta_ContaId",
                table: "PedidoConta",
                column: "ContaId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoConta_ProdutoId",
                table: "PedidoConta",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_TBConta_GarcomId",
                table: "TBConta",
                column: "GarcomId");

            migrationBuilder.CreateIndex(
                name: "IX_TBConta_MesaId",
                table: "TBConta",
                column: "MesaId");

            migrationBuilder.CreateIndex(
                name: "UQ_TBGarcom_UserId_Nome",
                table: "TBGarcom",
                columns: new[] { "UserId", "Nome" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PedidoConta");

            migrationBuilder.DropTable(
                name: "Produto");

            migrationBuilder.DropTable(
                name: "TBConta");

            migrationBuilder.DropTable(
                name: "TBGarcom");
        }
    }
}
