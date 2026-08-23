using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrewUp.Dashboards.ReadModel.Migrations
{
    /// <inheritdoc />
    public partial class MessagesReceived : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MessagesReceived",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessagesReceived", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessagesReceived",
                schema: "dbo");
        }
    }
}
