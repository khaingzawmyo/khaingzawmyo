using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Practise_project.Migrations
{
    /// <inheritdoc />
    public partial class AddUpdateDateAndEntryDateToPerson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /* 存在しない列の削除でエラーになるためコメントアウト
            migrationBuilder.DropColumn(
                name: "Create_person_id",
                table: "Invoice_item");
            */

            // 🆕 今回目的の Persons テーブルへの列追加だけを実行します
            migrationBuilder.AddColumn<DateTime>(
                name: "Entry_date",
                table: "Persons",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Update_date",
                table: "Persons",
                type: "datetime2",
                nullable: true);

            /* 他のテーブルへの不要な影響（エラー原因）をスキップするためにコメントアウト
            migrationBuilder.AddColumn<short>(
                name: "Rowver",
                table: "Invoice",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_item_Invoice_id",
                table: "Invoice_item",
                column: "Invoice_id");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_Create_person_id",
                table: "Invoice",
                column: "Create_person_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoice_Persons_Create_person_id",
                table: "Invoice",
                column: "Create_person_id",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoice_item_Invoice_Invoice_id",
                table: "Invoice_item",
                column: "Invoice_id",
                principalTable: "Invoice",
                principalColumn: "Invoice_id",
                onDelete: ReferentialAction.Cascade);
            */
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            /*
            migrationBuilder.DropForeignKey(
                name: "FK_Invoice_Persons_Create_person_id",
                table: "Invoice");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoice_item_Invoice_Invoice_id",
                table: "Invoice_item");

            migrationBuilder.DropIndex(
                name: "IX_Invoice_item_Invoice_id",
                table: "Invoice_item");

            migrationBuilder.DropIndex(
                name: "IX_Invoice_Create_person_id",
                table: "Invoice");
            */

            migrationBuilder.DropColumn(
                name: "Entry_date",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "Update_date",
                table: "Persons");

            /*
            migrationBuilder.DropColumn(
                name: "Rowver",
                table: "Invoice");
            */
        }
    }
}