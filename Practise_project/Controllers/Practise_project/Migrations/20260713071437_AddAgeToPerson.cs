using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Practise_project.Migrations
{
    /// <inheritdoc />
    public partial class AddAgeToPerson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Persons",
                newName: "Id");

            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "Persons",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Age",
                table: "Persons");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Persons",
                newName: "ID");
        }
    }
}
