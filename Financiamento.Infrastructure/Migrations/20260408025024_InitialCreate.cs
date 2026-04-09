using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Financiamento.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contratos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClienteCpfCnpj = table.Column<string>(type: "text", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TaxaMensal = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    PrazoMeses = table.Column<int>(type: "integer", nullable: false),
                    DataVencimentoPrimeiraParcela = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TipoVeiculo = table.Column<int>(type: "integer", nullable: false),
                    CondicaoVeiculo = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contratos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pagamentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ContratoId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParcelaNumero = table.Column<int>(type: "integer", nullable: false),
                    ValorPago = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DataPagamento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataVencimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pagamentos_Contratos_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "Contratos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pagamentos_ContratoId",
                table: "Pagamentos",
                column: "ContratoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pagamentos");

            migrationBuilder.DropTable(
                name: "Contratos");
        }
    }
}
