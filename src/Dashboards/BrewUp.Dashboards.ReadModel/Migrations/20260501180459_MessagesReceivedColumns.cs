using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrewUp.Dashboards.ReadModel.Migrations
{
    /// <inheritdoc />
    public partial class MessagesReceivedColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EntityName",
                schema: "dbo",
                table: "MessagesReceived",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ReceivedAt",
                schema: "dbo",
                table: "MessagesReceived",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EntityName",
                schema: "dbo",
                table: "MessagesReceived");

            migrationBuilder.DropColumn(
                name: "ReceivedAt",
                schema: "dbo",
                table: "MessagesReceived");
        }
    }
}
