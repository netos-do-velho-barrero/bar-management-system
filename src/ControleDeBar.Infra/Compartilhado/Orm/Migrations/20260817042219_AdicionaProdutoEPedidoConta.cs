using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleDeBar.Infra.Compartilhado.Orm.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaProdutoEPedidoConta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PedidoConta_Produto_ProdutoId",
                table: "PedidoConta");

            migrationBuilder.DropForeignKey(
                name: "FK_PedidoConta_TBConta_ContaId",
                table: "PedidoConta");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Produto",
                table: "Produto");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PedidoConta",
                table: "PedidoConta");

            migrationBuilder.RenameTable(
                name: "Produto",
                newName: "TBProduto");

            migrationBuilder.RenameTable(
                name: "PedidoConta",
                newName: "TBPedidoConta");

            migrationBuilder.RenameIndex(
                name: "IX_PedidoConta_ProdutoId",
                table: "TBPedidoConta",
                newName: "IX_TBPedidoConta_ProdutoId");

            migrationBuilder.RenameIndex(
                name: "IX_PedidoConta_ContaId",
                table: "TBPedidoConta",
                newName: "IX_TBPedidoConta_ContaId");

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "TBProduto",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TBProduto",
                table: "TBProduto",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TBPedidoConta",
                table: "TBPedidoConta",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "UQ_TBProduto_UserId_Nome",
                table: "TBProduto",
                columns: new[] { "UserId", "Nome" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TBPedidoConta_TBConta",
                table: "TBPedidoConta",
                column: "ContaId",
                principalTable: "TBConta",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TBPedidoConta_TBProduto",
                table: "TBPedidoConta",
                column: "ProdutoId",
                principalTable: "TBProduto",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TBPedidoConta_TBConta",
                table: "TBPedidoConta");

            migrationBuilder.DropForeignKey(
                name: "FK_TBPedidoConta_TBProduto",
                table: "TBPedidoConta");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TBProduto",
                table: "TBProduto");

            migrationBuilder.DropIndex(
                name: "UQ_TBProduto_UserId_Nome",
                table: "TBProduto");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TBPedidoConta",
                table: "TBPedidoConta");

            migrationBuilder.RenameTable(
                name: "TBProduto",
                newName: "Produto");

            migrationBuilder.RenameTable(
                name: "TBPedidoConta",
                newName: "PedidoConta");

            migrationBuilder.RenameIndex(
                name: "IX_TBPedidoConta_ProdutoId",
                table: "PedidoConta",
                newName: "IX_PedidoConta_ProdutoId");

            migrationBuilder.RenameIndex(
                name: "IX_TBPedidoConta_ContaId",
                table: "PedidoConta",
                newName: "IX_PedidoConta_ContaId");

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Produto",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Produto",
                table: "Produto",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PedidoConta",
                table: "PedidoConta",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PedidoConta_Produto_ProdutoId",
                table: "PedidoConta",
                column: "ProdutoId",
                principalTable: "Produto",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PedidoConta_TBConta_ContaId",
                table: "PedidoConta",
                column: "ContaId",
                principalTable: "TBConta",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
