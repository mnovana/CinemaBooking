using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ScreeningService.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScreeningRooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScreeningRooms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<int>(type: "int", nullable: false),
                    ScreeningRoomId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rows_ScreeningRooms_ScreeningRoomId",
                        column: x => x.ScreeningRoomId,
                        principalTable: "ScreeningRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Showtimes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Start = table.Column<DateTime>(type: "datetime2", nullable: false),
                    End = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ScreeningRoomId = table.Column<int>(type: "int", nullable: false),
                    MovieId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Showtimes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Showtimes_ScreeningRooms_ScreeningRoomId",
                        column: x => x.ScreeningRoomId,
                        principalTable: "ScreeningRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Seats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<int>(type: "int", nullable: false),
                    RowId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Seats_Rows_RowId",
                        column: x => x.RowId,
                        principalTable: "Rows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ScreeningRooms",
                columns: new[] { "Id", "Number" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 2 }
                });

            migrationBuilder.InsertData(
                table: "Rows",
                columns: new[] { "Id", "Number", "ScreeningRoomId" },
                values: new object[,]
                {
                    { 1, 1, 1 },
                    { 2, 2, 1 },
                    { 3, 3, 1 },
                    { 4, 1, 2 },
                    { 5, 2, 2 },
                    { 6, 3, 2 }
                });

            migrationBuilder.InsertData(
                table: "Showtimes",
                columns: new[] { "Id", "End", "MovieId", "ScreeningRoomId", "Start" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 10, 10, 19, 33, 0, 0, DateTimeKind.Unspecified), 1, 1, new DateTime(2024, 10, 10, 17, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, new DateTime(2024, 10, 11, 0, 3, 0, 0, DateTimeKind.Unspecified), 1, 1, new DateTime(2024, 10, 10, 21, 30, 0, 0, DateTimeKind.Unspecified) },
                    { 3, new DateTime(2024, 10, 10, 18, 51, 0, 0, DateTimeKind.Unspecified), 2, 2, new DateTime(2024, 10, 10, 17, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, new DateTime(2024, 10, 10, 23, 21, 0, 0, DateTimeKind.Unspecified), 2, 2, new DateTime(2024, 10, 10, 21, 30, 0, 0, DateTimeKind.Unspecified) },
                    { 5, new DateTime(2024, 10, 10, 18, 53, 0, 0, DateTimeKind.Unspecified), 3, 1, new DateTime(2024, 10, 11, 17, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6, new DateTime(2024, 10, 10, 18, 53, 0, 0, DateTimeKind.Unspecified), 3, 2, new DateTime(2024, 10, 11, 17, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Seats",
                columns: new[] { "Id", "Number", "RowId" },
                values: new object[,]
                {
                    { 1, 1, 1 },
                    { 2, 2, 1 },
                    { 3, 3, 1 },
                    { 4, 4, 1 },
                    { 5, 1, 2 },
                    { 6, 2, 2 },
                    { 7, 3, 2 },
                    { 8, 4, 2 },
                    { 9, 1, 3 },
                    { 10, 2, 3 },
                    { 11, 3, 3 },
                    { 12, 4, 3 },
                    { 13, 1, 4 },
                    { 14, 2, 4 },
                    { 15, 3, 4 },
                    { 16, 4, 4 },
                    { 17, 1, 5 },
                    { 18, 2, 5 },
                    { 19, 3, 5 },
                    { 20, 4, 5 },
                    { 21, 1, 6 },
                    { 22, 2, 6 },
                    { 23, 3, 6 },
                    { 24, 4, 6 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rows_Number_ScreeningRoomId",
                table: "Rows",
                columns: new[] { "Number", "ScreeningRoomId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rows_ScreeningRoomId",
                table: "Rows",
                column: "ScreeningRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_ScreeningRooms_Number",
                table: "ScreeningRooms",
                column: "Number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Seats_Number_RowId",
                table: "Seats",
                columns: new[] { "Number", "RowId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Seats_RowId",
                table: "Seats",
                column: "RowId");

            migrationBuilder.CreateIndex(
                name: "IX_Showtimes_ScreeningRoomId",
                table: "Showtimes",
                column: "ScreeningRoomId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Seats");

            migrationBuilder.DropTable(
                name: "Showtimes");

            migrationBuilder.DropTable(
                name: "Rows");

            migrationBuilder.DropTable(
                name: "ScreeningRooms");
        }
    }
}
