using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InnHotel.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRoomStatusConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_rooms_status",
                table: "rooms");

            migrationBuilder.AddCheckConstraint(
                name: "CK_rooms_status",
                table: "rooms",
                sql: "status IN ('Available','Occupied','UnderMaintenance')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_rooms_status",
                table: "rooms");

            migrationBuilder.AddCheckConstraint(
                name: "CK_rooms_status",
                table: "rooms",
                sql: "status IN ('Available','Occupied','Under Maintenance')");
        }
    }
}
