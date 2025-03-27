using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestProjectAnnur.Migrations
{
    /// <inheritdoc />
    public partial class Update5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Certificate",
                table: "RegisterEvents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IsAttend",
                table: "RegisterEvents",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Certificate",
                table: "RegisterEvents");

            migrationBuilder.DropColumn(
                name: "IsAttend",
                table: "RegisterEvents");
        }
    }
}
