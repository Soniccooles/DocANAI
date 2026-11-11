using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DocANAI.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTxtAndMdDocumentFormats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "format",
                columns: new[] { "extension", "max_size", "mime_type" },
                values: new object[,]
                {
                    { ".md", 104857600, "text/markdown" },
                    { ".txt", 104857600, "text/plain" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "format",
                keyColumn: "extension",
                keyValue: ".md");

            migrationBuilder.DeleteData(
                table: "format",
                keyColumn: "extension",
                keyValue: ".txt");
        }
    }
}
