﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookify.Web.Data.Migrations
{
    public partial class AddCreatedByAndLastUpdatedByToUsersTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Categories",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastUpdatedById",
                table: "Categories",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Books",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastUpdatedById",
                table: "Books",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "BookCopy",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastUpdatedById",
                table: "BookCopy",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Authors",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastUpdatedById",
                table: "Authors",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastUpdatedById",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_CreatedById",
                table: "Categories",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_LastUpdatedById",
                table: "Categories",
                column: "LastUpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Books_CreatedById",
                table: "Books",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Books_LastUpdatedById",
                table: "Books",
                column: "LastUpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_BookCopy_CreatedById",
                table: "BookCopy",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_BookCopy_LastUpdatedById",
                table: "BookCopy",
                column: "LastUpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Authors_CreatedById",
                table: "Authors",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Authors_LastUpdatedById",
                table: "Authors",
                column: "LastUpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Authors_AspNetUsers_CreatedById",
                table: "Authors",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Authors_AspNetUsers_LastUpdatedById",
                table: "Authors",
                column: "LastUpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BookCopy_AspNetUsers_CreatedById",
                table: "BookCopy",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BookCopy_AspNetUsers_LastUpdatedById",
                table: "BookCopy",
                column: "LastUpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_AspNetUsers_CreatedById",
                table: "Books",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_AspNetUsers_LastUpdatedById",
                table: "Books",
                column: "LastUpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_AspNetUsers_CreatedById",
                table: "Categories",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_AspNetUsers_LastUpdatedById",
                table: "Categories",
                column: "LastUpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Authors_AspNetUsers_CreatedById",
                table: "Authors");

            migrationBuilder.DropForeignKey(
                name: "FK_Authors_AspNetUsers_LastUpdatedById",
                table: "Authors");

            migrationBuilder.DropForeignKey(
                name: "FK_BookCopy_AspNetUsers_CreatedById",
                table: "BookCopy");

            migrationBuilder.DropForeignKey(
                name: "FK_BookCopy_AspNetUsers_LastUpdatedById",
                table: "BookCopy");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_AspNetUsers_CreatedById",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_AspNetUsers_LastUpdatedById",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Categories_AspNetUsers_CreatedById",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Categories_AspNetUsers_LastUpdatedById",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_CreatedById",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_LastUpdatedById",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Books_CreatedById",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_LastUpdatedById",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_BookCopy_CreatedById",
                table: "BookCopy");

            migrationBuilder.DropIndex(
                name: "IX_BookCopy_LastUpdatedById",
                table: "BookCopy");

            migrationBuilder.DropIndex(
                name: "IX_Authors_CreatedById",
                table: "Authors");

            migrationBuilder.DropIndex(
                name: "IX_Authors_LastUpdatedById",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "LastUpdatedById",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "LastUpdatedById",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "BookCopy");

            migrationBuilder.DropColumn(
                name: "LastUpdatedById",
                table: "BookCopy");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "LastUpdatedById",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastUpdatedById",
                table: "AspNetUsers");
        }
    }
}
