using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Practise_project.Migrations
{
    /// <inheritdoc />
    public partial class New_Tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SurName",
                table: "Persons",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PersonType",
                table: "Persons",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "GivenName",
                table: "Persons",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Age",
                table: "Persons",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

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
                    Update_date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoice", x => x.Invoice_id);
                });

            migrationBuilder.CreateTable(
                name: "Invoice_item",
                columns: table => new
                {
                    Invoice_item_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Invoice_id = table.Column<int>(type: "int", nullable: false),
                    Charge_description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Create_person_id = table.Column<int>(type: "int", nullable: false),
                    Revenue_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Cost_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Entry_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Update_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Rowver = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoice_item", x => x.Invoice_item_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Invoice");

            migrationBuilder.DropTable(
                name: "Invoice_item");

            migrationBuilder.AlterColumn<string>(
                name: "SurName",
                table: "Persons",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PersonType",
                table: "Persons",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "GivenName",
                table: "Persons",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Age",
                table: "Persons",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
