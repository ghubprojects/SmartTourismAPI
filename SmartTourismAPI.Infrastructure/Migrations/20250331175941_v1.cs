using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartTourismAPI.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class v1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "m_place_detail",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "type_id",
                table: "m_place_detail",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_m_place_detail_type_id",
                table: "m_place_detail",
                column: "type_id");

            migrationBuilder.AddForeignKey(
                name: "m_place_detail_m_place_type_fk",
                table: "m_place_detail",
                column: "type_id",
                principalTable: "m_place_type",
                principalColumn: "type_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "m_place_detail_m_place_type_fk",
                table: "m_place_detail");

            migrationBuilder.DropIndex(
                name: "IX_m_place_detail_type_id",
                table: "m_place_detail");

            migrationBuilder.DropColumn(
                name: "name",
                table: "m_place_detail");

            migrationBuilder.DropColumn(
                name: "type_id",
                table: "m_place_detail");
        }
    }
}
