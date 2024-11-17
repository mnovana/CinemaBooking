using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SeatReservationService.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ShowtimeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReservedSeats",
                columns: table => new
                {
                    ReservationId = table.Column<int>(type: "int", nullable: false),
                    SeatId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservedSeats", x => new { x.ReservationId, x.SeatId });
                    table.ForeignKey(
                        name: "FK_ReservedSeats_Reservations_ReservationId",
                        column: x => x.ReservationId,
                        principalTable: "Reservations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Reservations",
                columns: new[] { "Id", "ShowtimeId", "UserEmail" },
                values: new object[,]
                {
                    { 1, 1, "novana@gmail.com" },
                    { 2, 2, "novana@gmail.com" },
                    { 3, 3, "novana@gmail.com" },
                    { 4, 4, "novana@gmail.com" }
                });

            migrationBuilder.InsertData(
                table: "ReservedSeats",
                columns: new[] { "ReservationId", "SeatId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 2, 1 },
                    { 2, 2 },
                    { 3, 14 },
                    { 3, 15 },
                    { 4, 14 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReservedSeats");

            migrationBuilder.DropTable(
                name: "Reservations");
        }
    }
}
