using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PuzzleManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PuzzleHolders",
                columns: table => new
                {
                    PuzzleHolderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Contact = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PuzzleHolders", x => x.PuzzleHolderId);
                });

            migrationBuilder.CreateTable(
                name: "PuzzleMakers",
                columns: table => new
                {
                    PuzzleMakerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PuzzleMakers", x => x.PuzzleMakerId);
                });

            migrationBuilder.CreateTable(
                name: "Puzzles",
                columns: table => new
                {
                    PuzzleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PieceCount = table.Column<int>(type: "int", nullable: false),
                    DifficultyRating = table.Column<double>(type: "float", nullable: false),
                    PuzzleMakerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Puzzles", x => x.PuzzleId);
                    table.ForeignKey(
                        name: "FK_Puzzles_PuzzleMakers_PuzzleMakerId",
                        column: x => x.PuzzleMakerId,
                        principalTable: "PuzzleMakers",
                        principalColumn: "PuzzleMakerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PuzzleCheckouts",
                columns: table => new
                {
                    PuzzleCheckoutId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PuzzleId = table.Column<int>(type: "int", nullable: false),
                    PuzzleHolderId = table.Column<int>(type: "int", nullable: false),
                    CheckoutDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReturnDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TimeToComplete = table.Column<double>(type: "float", nullable: true),
                    UserDifficultyRating = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PuzzleCheckouts", x => x.PuzzleCheckoutId);
                    table.ForeignKey(
                        name: "FK_PuzzleCheckouts_PuzzleHolders_PuzzleHolderId",
                        column: x => x.PuzzleHolderId,
                        principalTable: "PuzzleHolders",
                        principalColumn: "PuzzleHolderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PuzzleCheckouts_Puzzles_PuzzleId",
                        column: x => x.PuzzleId,
                        principalTable: "Puzzles",
                        principalColumn: "PuzzleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PuzzleCheckouts_PuzzleHolderId",
                table: "PuzzleCheckouts",
                column: "PuzzleHolderId");

            migrationBuilder.CreateIndex(
                name: "IX_PuzzleCheckouts_PuzzleId",
                table: "PuzzleCheckouts",
                column: "PuzzleId");

            migrationBuilder.CreateIndex(
                name: "IX_Puzzles_PuzzleMakerId",
                table: "Puzzles",
                column: "PuzzleMakerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PuzzleCheckouts");

            migrationBuilder.DropTable(
                name: "PuzzleHolders");

            migrationBuilder.DropTable(
                name: "Puzzles");

            migrationBuilder.DropTable(
                name: "PuzzleMakers");
        }
    }
}
