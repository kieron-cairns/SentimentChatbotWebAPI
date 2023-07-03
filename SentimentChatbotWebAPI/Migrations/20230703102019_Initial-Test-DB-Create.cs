using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SentimentChatbotWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialTestDBCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QueryHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(16)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    QueryText = table.Column<string>(type: "nvarchar(300)", nullable: false),
                    QueryResult = table.Column<string>(type: "nvarchar(8)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueryHistories", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QueryHistories");
        }
    }
}
