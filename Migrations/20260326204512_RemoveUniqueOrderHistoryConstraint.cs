using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend_purchase_order.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUniqueOrderHistoryConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrderActionHistory_UserId_OrderId",
                table: "OrderActionHistory");

            migrationBuilder.CreateIndex(
                name: "IX_OrderActionHistory_UserId_OrderId",
                table: "OrderActionHistory",
                columns: new[] { "UserId", "OrderId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrderActionHistory_UserId_OrderId",
                table: "OrderActionHistory");

            migrationBuilder.CreateIndex(
                name: "IX_OrderActionHistory_UserId_OrderId",
                table: "OrderActionHistory",
                columns: new[] { "UserId", "OrderId" },
                unique: true);
        }
    }
}
