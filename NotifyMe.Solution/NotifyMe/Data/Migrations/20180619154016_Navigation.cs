using Microsoft.EntityFrameworkCore.Migrations;

namespace NotifyMe.Migrations
{
    public partial class Navigation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Connections_Users_UserId",
                table: "Connections");

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "Connections",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Connections_Users_UserId",
                table: "Connections",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Connections_Users_UserId",
                table: "Connections");

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "Connections",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddForeignKey(
                name: "FK_Connections_Users_UserId",
                table: "Connections",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
