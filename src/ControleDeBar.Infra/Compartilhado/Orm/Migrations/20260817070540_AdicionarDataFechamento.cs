using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleDeBar.Infra.Compartilhado.Orm.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarDataFechamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataFechamento",
                table: "TBConta",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataFechamento",
                table: "TBConta");
        }
    }
}
