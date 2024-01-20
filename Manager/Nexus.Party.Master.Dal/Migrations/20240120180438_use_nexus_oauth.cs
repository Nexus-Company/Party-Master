using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nexus.Party.Master.Dal.Migrations
{
    /// <inheritdoc />
    public partial class use_nexus_oauth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GoogleId",
                table: "Accounts",
                newName: "NexusId");

            migrationBuilder.AddColumn<double>(
                name: "MaxAge",
                table: "Authentications",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Refresh",
                table: "Authentications",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxAge",
                table: "Authentications");

            migrationBuilder.DropColumn(
                name: "Refresh",
                table: "Authentications");

            migrationBuilder.RenameColumn(
                name: "NexusId",
                table: "Accounts",
                newName: "GoogleId");
        }
    }
}
