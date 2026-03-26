using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend_purchase_order.Migrations
{
    /// <inheritdoc />
    public partial class RestrictUserOrderHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderActionHistory_User_UserId",
                table: "OrderActionHistory");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderActionHistory_User_UserId",
                table: "OrderActionHistory",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderActionHistory_User_UserId",
                table: "OrderActionHistory");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderActionHistory_User_UserId",
                table: "OrderActionHistory",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
