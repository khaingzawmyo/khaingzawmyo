using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Practise_project.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GivenName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SurName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LocalLanguageName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PersonType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Age = table.Column<int>(type: "int", nullable: false),
                    Entry_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Update_date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Invoice",
                columns: table => new
                {
                    Invoice_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Invoice_no = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Create_person_id = table.Column<int>(type: "int", nullable: false),
                    Total_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Customer_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Void_flag = table.Column<bool>(type: "bit", nullable: false),
                    Entry_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Update_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Rowver = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoice", x => x.Invoice_id);
                    table.ForeignKey(
                        name: "FK_Invoice_Persons_Create_person_id",
                        column: x => x.Create_person_id,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Invoice_item",
                columns: table => new
                {
                    Invoice_item_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Invoice_id = table.Column<int>(type: "int", nullable: false),
                    Charge_description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Revenue_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Cost_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Entry_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Update_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Rowver = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoice_item", x => x.Invoice_item_id);
                    table.ForeignKey(
                        name: "FK_Invoice_item_Invoice_Invoice_id",
                        column: x => x.Invoice_id,
                        principalTable: "Invoice",
                        principalColumn: "Invoice_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_Create_person_id",
                table: "Invoice",
                column: "Create_person_id");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_item_Invoice_id",
                table: "Invoice_item",
                column: "Invoice_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Invoice_item");

            migrationBuilder.DropTable(
                name: "Invoice");

            migrationBuilder.DropTable(
                name: "Persons");
        }
    }
}
