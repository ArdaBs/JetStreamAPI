using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace JetStreamAPI.Migrations
{
    /// <inheritdoc />
    public partial class JetStreamMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE LOGIN JetStreamAdmin WITH PASSWORD = 'Password1234!';");
            migrationBuilder.Sql("USE JetStream_Backend;");
            migrationBuilder.Sql("CREATE USER JetStreamAdmin FOR LOGIN JetStreamAdmin;");
            migrationBuilder.Sql("GRANT SELECT, INSERT, UPDATE, DELETE TO JetStreamAdmin;");

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    FailedLoginAttempts = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServiceTypeId = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PickupDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceOrders_ServiceTypes_ServiceTypeId",
                        column: x => x.ServiceTypeId,
                        principalTable: "ServiceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "FailedLoginAttempts", "IsLocked", "Password", "Username" },
                values: new object[,]
                {
                    { 1, 0, false, "1234", "Arda" },
                    { 2, 0, false, "1234", "Satoru" },
                    { 3, 0, false, "1234", "Smith" },
                    { 4, 0, false, "1234", "Lukas" },
                    { 5, 0, false, "1234", "Daniel" },
                    { 6, 0, false, "1234", "Tobey" },
                    { 7, 0, false, "1234", "Micheal" },
                    { 8, 0, false, "1234", "Brian" },
                    { 9, 0, false, "1234", "Alim" },
                    { 10, 0, false, "1234", "Sven" }
                });

            migrationBuilder.InsertData(
                table: "ServiceTypes",
                columns: new[] { "Id", "Cost", "Name" },
                values: new object[,]
                {
                    { 1, 34.95m, "Kleiner Service" },
                    { 2, 59.95m, "Grosser Service" },
                    { 3, 74.95m, "Rennski-Service" },
                    { 4, 24.95m, "Bindung montieren und einstellen" },
                    { 5, 14.95m, "Fell zuschneiden" },
                    { 6, 19.95m, "Heisswachsen" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOrders_ServiceTypeId",
                table: "ServiceOrders",
                column: "ServiceTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP USER JetStreamAdmin;");
            migrationBuilder.Sql("DROP LOGIN JetStreamAdmin;");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "ServiceOrders");

            migrationBuilder.DropTable(
                name: "ServiceTypes");
        }
    }
}
