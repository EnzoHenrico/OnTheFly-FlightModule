using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlightsApi.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    ZipCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Number = table.Column<int>(type: "int", nullable: false),
                    Street = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Complement = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => new { x.ZipCode, x.Number });
                });

            migrationBuilder.CreateTable(
                name: "Airport",
                columns: table => new
                {
                    _id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Iata = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Airport", x => x._id);
                });

            migrationBuilder.CreateTable(
                name: "Company",
                columns: table => new
                {
                    Cnpj = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NameOpt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OpeningDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: true),
                    AddressZipCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AddressNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company", x => new { x.Cnpj, x.Name });
                    table.ForeignKey(
                        name: "FK_Company_Address_AddressZipCode_AddressNumber",
                        columns: x => new { x.AddressZipCode, x.AddressNumber },
                        principalTable: "Address",
                        principalColumns: new[] { "ZipCode", "Number" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Aircraft",
                columns: table => new
                {
                    Rab = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    RegistryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastFlightDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompanyCnpj = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aircraft", x => x.Rab);
                    table.ForeignKey(
                        name: "FK_Aircraft_Company_CompanyCnpj_CompanyName",
                        columns: x => new { x.CompanyCnpj, x.CompanyName },
                        principalTable: "Company",
                        principalColumns: new[] { "Cnpj", "Name" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Flight",
                columns: table => new
                {
                    FlightNumber = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Arrival_id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PlaneRab = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Schedule = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Sales = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flight", x => x.FlightNumber);
                    table.ForeignKey(
                        name: "FK_Flight_Aircraft_PlaneRab",
                        column: x => x.PlaneRab,
                        principalTable: "Aircraft",
                        principalColumn: "Rab");
                    table.ForeignKey(
                        name: "FK_Flight_Airport_Arrival_id",
                        column: x => x.Arrival_id,
                        principalTable: "Airport",
                        principalColumn: "_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Aircraft_CompanyCnpj_CompanyName",
                table: "Aircraft",
                columns: new[] { "CompanyCnpj", "CompanyName" });

            migrationBuilder.CreateIndex(
                name: "IX_Company_AddressZipCode_AddressNumber",
                table: "Company",
                columns: new[] { "AddressZipCode", "AddressNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Flight_Arrival_id",
                table: "Flight",
                column: "Arrival_id");

            migrationBuilder.CreateIndex(
                name: "IX_Flight_PlaneRab",
                table: "Flight",
                column: "PlaneRab");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Flight");

            migrationBuilder.DropTable(
                name: "Aircraft");

            migrationBuilder.DropTable(
                name: "Airport");

            migrationBuilder.DropTable(
                name: "Company");

            migrationBuilder.DropTable(
                name: "Address");
        }
    }
}
