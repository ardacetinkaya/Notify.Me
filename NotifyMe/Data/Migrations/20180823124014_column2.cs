using Microsoft.EntityFrameworkCore.Migrations;

namespace NotifyMe.Migrations
{
    public partial class column2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RawContent",
                table: "Message",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RawContent",
                table: "Message");
        }
    }
}
