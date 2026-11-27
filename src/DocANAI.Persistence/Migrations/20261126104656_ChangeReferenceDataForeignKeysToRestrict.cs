using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocANAI.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeReferenceDataForeignKeysToRestrict : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_tasks_model",
                table: "processing_task");

            migrationBuilder.DropForeignKey(
                name: "fk_tasks_priority",
                table: "processing_task");

            migrationBuilder.DropForeignKey(
                name: "FK_question_file_format_extension",
                table: "question_file");

            migrationBuilder.DropForeignKey(
                name: "FK_report_format_extension",
                table: "report");

            migrationBuilder.DropForeignKey(
                name: "FK_source_document_format_extension",
                table: "source_document");

            migrationBuilder.AddForeignKey(
                name: "fk_tasks_model",
                table: "processing_task",
                column: "model_id",
                principalTable: "a_i_model",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_tasks_priority",
                table: "processing_task",
                column: "priority_id",
                principalTable: "priority",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_question_file_format_extension",
                table: "question_file",
                column: "extension",
                principalTable: "format",
                principalColumn: "extension",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_report_format_extension",
                table: "report",
                column: "extension",
                principalTable: "format",
                principalColumn: "extension",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_source_document_format_extension",
                table: "source_document",
                column: "extension",
                principalTable: "format",
                principalColumn: "extension",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_tasks_model",
                table: "processing_task");

            migrationBuilder.DropForeignKey(
                name: "fk_tasks_priority",
                table: "processing_task");

            migrationBuilder.DropForeignKey(
                name: "FK_question_file_format_extension",
                table: "question_file");

            migrationBuilder.DropForeignKey(
                name: "FK_report_format_extension",
                table: "report");

            migrationBuilder.DropForeignKey(
                name: "FK_source_document_format_extension",
                table: "source_document");

            migrationBuilder.AddForeignKey(
                name: "fk_tasks_model",
                table: "processing_task",
                column: "model_id",
                principalTable: "a_i_model",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_tasks_priority",
                table: "processing_task",
                column: "priority_id",
                principalTable: "priority",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_question_file_format_extension",
                table: "question_file",
                column: "extension",
                principalTable: "format",
                principalColumn: "extension",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_report_format_extension",
                table: "report",
                column: "extension",
                principalTable: "format",
                principalColumn: "extension",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_source_document_format_extension",
                table: "source_document",
                column: "extension",
                principalTable: "format",
                principalColumn: "extension",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
