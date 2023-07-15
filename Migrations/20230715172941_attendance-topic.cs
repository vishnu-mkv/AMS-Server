using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMS.Migrations
{
    /// <inheritdoc />
    public partial class attendancetopic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TopicId",
                table: "Attendances",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_TopicId",
                table: "Attendances",
                column: "TopicId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Topics_TopicId",
                table: "Attendances",
                column: "TopicId",
                principalTable: "Topics",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Topics_TopicId",
                table: "Attendances");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_TopicId",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "TopicId",
                table: "Attendances");
        }
    }
}
