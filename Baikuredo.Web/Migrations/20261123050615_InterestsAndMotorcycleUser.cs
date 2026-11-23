using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Baikuredo.Web.Migrations
{
    /// <inheritdoc />
    public partial class InterestsAndMotorcycleUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Motorcycles",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Interests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MotorcyclePublicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsAttended = table.Column<bool>(type: "bit", nullable: false),
                    AttendedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AttendedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Interests_AspNetUsers_AttendedByUserId",
                        column: x => x.AttendedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Interests_MotorcyclePublications_MotorcyclePublicationId",
                        column: x => x.MotorcyclePublicationId,
                        principalTable: "MotorcyclePublications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Motorcycles_UserId",
                table: "Motorcycles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Interests_AttendedByUserId",
                table: "Interests",
                column: "AttendedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Interests_CreatedAt",
                table: "Interests",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Interests_MotorcyclePublicationId",
                table: "Interests",
                column: "MotorcyclePublicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Motorcycles_AspNetUsers_UserId",
                table: "Motorcycles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Motorcycles_AspNetUsers_UserId",
                table: "Motorcycles");

            migrationBuilder.DropTable(
                name: "Interests");

            migrationBuilder.DropIndex(
                name: "IX_Motorcycles_UserId",
                table: "Motorcycles");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Motorcycles");
        }
    }
}
