using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMS.Migrations
{
    /// <inheritdoc />
    public partial class schedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ScheduleId",
                table: "Users",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Schedules",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrganizationId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Schedules_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_ScheduleId",
                table: "Users",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_OrganizationId",
                table: "Schedules",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Schedules_ScheduleId",
                table: "Users",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Schedules_ScheduleId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Users_ScheduleId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ScheduleId",
                table: "Users");
        }
    }
}
