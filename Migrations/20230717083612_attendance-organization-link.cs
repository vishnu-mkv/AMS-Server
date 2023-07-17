using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMS.Migrations
{
    /// <inheritdoc />
    public partial class attendanceorganizationlink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrganizationId",
                table: "Attendances",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_OrganizationId",
                table: "Attendances",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Organizations_OrganizationId",
                table: "Attendances",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Organizations_OrganizationId",
                table: "Attendances");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_OrganizationId",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "Attendances");
        }
    }
}
