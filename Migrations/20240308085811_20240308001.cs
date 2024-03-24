using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BSBoilerPlate.Migrations
{
    public partial class _20240308001 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserFromID = table.Column<int>(type: "int", nullable: true),
                    UserToID = table.Column<int>(type: "int", nullable: true),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateTimeRead = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ChatMessages_Users_UserFromID",
                        column: x => x.UserFromID,
                        principalTable: "Users",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_ChatMessages_Users_UserToID",
                        column: x => x.UserToID,
                        principalTable: "Users",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_UserFromID",
                table: "ChatMessages",
                column: "UserFromID");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_UserToID",
                table: "ChatMessages",
                column: "UserToID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatMessages");
        }
    }
}
