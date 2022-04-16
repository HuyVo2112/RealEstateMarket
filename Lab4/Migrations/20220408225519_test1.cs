using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Assignment2.Migrations
{
    public partial class test1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BrokerageId",
                table: "Advertisement",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Advertisement_BrokerageId",
                table: "Advertisement",
                column: "BrokerageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Advertisement_Brokerage_BrokerageId",
                table: "Advertisement",
                column: "BrokerageId",
                principalTable: "Brokerage",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Advertisement_Brokerage_BrokerageId",
                table: "Advertisement");

            migrationBuilder.DropIndex(
                name: "IX_Advertisement_BrokerageId",
                table: "Advertisement");

            migrationBuilder.DropColumn(
                name: "BrokerageId",
                table: "Advertisement");
        }
    }
}
