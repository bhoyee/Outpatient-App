using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Outpatient_App.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedPateintTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Postcode",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Postcode",
                table: "Patients");
        }
    }
}
