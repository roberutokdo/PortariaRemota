using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PortariaRemotaAPI.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Apartamentos",
                columns: table => new
                {
                    ApartamentoId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Bloco = table.Column<string>(nullable: true),
                    Numero = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Apartamentos", x => x.ApartamentoId);
                });

            migrationBuilder.CreateTable(
                name: "Logins",
                columns: table => new
                {
                    LoginId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    User = table.Column<string>(nullable: false),
                    Pass = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logins", x => x.LoginId);
                });

            migrationBuilder.CreateTable(
                name: "Moradores",
                columns: table => new
                {
                    MoradorId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(nullable: true),
                    DataNascimento = table.Column<DateTime>(type: "Date", nullable: false),
                    Telefone = table.Column<string>(nullable: true),
                    CPF = table.Column<string>(nullable: true),
                    EMail = table.Column<string>(nullable: true),
                    ApartamentoId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Moradores", x => x.MoradorId);
                    table.ForeignKey(
                        name: "FK_Moradores_Apartamentos_ApartamentoId",
                        column: x => x.ApartamentoId,
                        principalTable: "Apartamentos",
                        principalColumn: "ApartamentoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Logins",
                columns: new[] { "LoginId", "Pass", "User" },
                values: new object[] { 1, "Kiper@2018", "Kiper" });

            migrationBuilder.CreateIndex(
                name: "IX_Moradores_ApartamentoId",
                table: "Moradores",
                column: "ApartamentoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Logins");

            migrationBuilder.DropTable(
                name: "Moradores");

            migrationBuilder.DropTable(
                name: "Apartamentos");
        }
    }
}
