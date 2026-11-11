using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DocANAI.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:vector", ",,");

            migrationBuilder.CreateTable(
                name: "a_i_model",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    version = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    is_available = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_a_i_model", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "format",
                columns: table => new
                {
                    extension = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    max_size = table.Column<int>(type: "integer", nullable: false),
                    mime_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_format", x => x.extension);
                });

            migrationBuilder.CreateTable(
                name: "priority_level",
                columns: table => new
                {
                    level = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    weight = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_priority_level", x => x.level);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    user_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "priority",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    priority_level = table.Column<string>(type: "character varying(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_priority", x => x.id);
                    table.ForeignKey(
                        name: "FK_priority_priority_level_priority_level",
                        column: x => x.priority_level,
                        principalTable: "priority_level",
                        principalColumn: "level",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "refresh_token",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRevoked = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedByIp = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    RevokedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RevokedByIp = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_token", x => x.Id);
                    table.ForeignKey(
                        name: "FK_refresh_token_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "processing_task",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    model_id = table.Column<Guid>(type: "uuid", nullable: false),
                    priority_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    start_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    end_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_processing_task", x => x.id);
                    table.ForeignKey(
                        name: "fk_tasks_model",
                        column: x => x.model_id,
                        principalTable: "a_i_model",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_tasks_priority",
                        column: x => x.priority_id,
                        principalTable: "priority",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_tasks_user",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "question_file",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    task_id = table.Column<Guid>(type: "uuid", nullable: false),
                    extension = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    filename = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    file_path = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_question_file", x => x.id);
                    table.ForeignKey(
                        name: "FK_question_file_format_extension",
                        column: x => x.extension,
                        principalTable: "format",
                        principalColumn: "extension",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_question_file_processing_task_task_id",
                        column: x => x.task_id,
                        principalTable: "processing_task",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_question_file_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "report",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    task_id = table.Column<Guid>(type: "uuid", nullable: false),
                    extension = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    file_path = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    deletion_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_report", x => x.id);
                    table.ForeignKey(
                        name: "FK_report_format_extension",
                        column: x => x.extension,
                        principalTable: "format",
                        principalColumn: "extension",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_report_processing_task_task_id",
                        column: x => x.task_id,
                        principalTable: "processing_task",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "source_document",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    task_id = table.Column<Guid>(type: "uuid", nullable: false),
                    extension = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    filename = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    file_path = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    size = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_source_document", x => x.id);
                    table.ForeignKey(
                        name: "FK_source_document_format_extension",
                        column: x => x.extension,
                        principalTable: "format",
                        principalColumn: "extension",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_source_document_processing_task_task_id",
                        column: x => x.task_id,
                        principalTable: "processing_task",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_source_document_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "question",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    task_id = table.Column<Guid>(type: "uuid", nullable: false),
                    question_file_id = table.Column<Guid>(type: "uuid", nullable: true),
                    question_number = table.Column<int>(type: "integer", nullable: false),
                    text = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_question", x => x.id);
                    table.ForeignKey(
                        name: "FK_question_processing_task_task_id",
                        column: x => x.task_id,
                        principalTable: "processing_task",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_question_question_file_question_file_id",
                        column: x => x.question_file_id,
                        principalTable: "question_file",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "document_chunk",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_document_id = table.Column<Guid>(type: "uuid", nullable: false),
                    chunk_index = table.Column<int>(type: "integer", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    page_number = table.Column<int>(type: "integer", nullable: true),
                    embedding = table.Column<Vector>(type: "vector(768)", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_document_chunk", x => x.id);
                    table.ForeignKey(
                        name: "FK_document_chunk_source_document_source_document_id",
                        column: x => x.source_document_id,
                        principalTable: "source_document",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "answer",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    question_id = table.Column<Guid>(type: "uuid", nullable: false),
                    task_id = table.Column<Guid>(type: "uuid", nullable: false),
                    text = table.Column<string>(type: "text", nullable: false),
                    model_accuracy = table.Column<decimal>(type: "numeric(4,2)", precision: 4, scale: 2, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_answer", x => x.id);
                    table.ForeignKey(
                        name: "FK_answer_processing_task_task_id",
                        column: x => x.task_id,
                        principalTable: "processing_task",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_answer_question_question_id",
                        column: x => x.question_id,
                        principalTable: "question",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "format",
                columns: new[] { "extension", "max_size", "mime_type" },
                values: new object[,]
                {
                    { ".docx", 104857600, "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
                    { ".pdf", 104857600, "application/pdf" },
                    { ".xlsx", 104857600, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_a_i_model_name_version",
                table: "a_i_model",
                columns: new[] { "name", "version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_answer_question_id",
                table: "answer",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "IX_answer_task_id",
                table: "answer",
                column: "task_id");

            migrationBuilder.CreateIndex(
                name: "IX_document_chunk_embedding",
                table: "document_chunk",
                column: "embedding")
                .Annotation("Npgsql:IndexMethod", "hnsw")
                .Annotation("Npgsql:IndexOperators", new[] { "vector_cosine_ops" })
                .Annotation("Npgsql:StorageParameter:ef_construction", 64)
                .Annotation("Npgsql:StorageParameter:m", 16);

            migrationBuilder.CreateIndex(
                name: "IX_document_chunk_source_document_id_chunk_index",
                table: "document_chunk",
                columns: new[] { "source_document_id", "chunk_index" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_priority_priority_level",
                table: "priority",
                column: "priority_level");

            migrationBuilder.CreateIndex(
                name: "IX_processing_task_model_id",
                table: "processing_task",
                column: "model_id");

            migrationBuilder.CreateIndex(
                name: "IX_processing_task_priority_id",
                table: "processing_task",
                column: "priority_id");

            migrationBuilder.CreateIndex(
                name: "IX_processing_task_user_id",
                table: "processing_task",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_question_question_file_id",
                table: "question",
                column: "question_file_id");

            migrationBuilder.CreateIndex(
                name: "IX_question_task_id",
                table: "question",
                column: "task_id");

            migrationBuilder.CreateIndex(
                name: "IX_question_file_extension",
                table: "question_file",
                column: "extension");

            migrationBuilder.CreateIndex(
                name: "IX_question_file_task_id",
                table: "question_file",
                column: "task_id");

            migrationBuilder.CreateIndex(
                name: "IX_question_file_user_id",
                table: "question_file",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_token_Token",
                table: "refresh_token",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_refresh_token_UserId",
                table: "refresh_token",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_report_extension",
                table: "report",
                column: "extension");

            migrationBuilder.CreateIndex(
                name: "IX_report_task_id",
                table: "report",
                column: "task_id");

            migrationBuilder.CreateIndex(
                name: "IX_source_document_extension",
                table: "source_document",
                column: "extension");

            migrationBuilder.CreateIndex(
                name: "IX_source_document_task_id",
                table: "source_document",
                column: "task_id");

            migrationBuilder.CreateIndex(
                name: "IX_source_document_user_id",
                table: "source_document",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "answer");

            migrationBuilder.DropTable(
                name: "document_chunk");

            migrationBuilder.DropTable(
                name: "refresh_token");

            migrationBuilder.DropTable(
                name: "report");

            migrationBuilder.DropTable(
                name: "question");

            migrationBuilder.DropTable(
                name: "source_document");

            migrationBuilder.DropTable(
                name: "question_file");

            migrationBuilder.DropTable(
                name: "format");

            migrationBuilder.DropTable(
                name: "processing_task");

            migrationBuilder.DropTable(
                name: "a_i_model");

            migrationBuilder.DropTable(
                name: "priority");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.DropTable(
                name: "priority_level");
        }
    }
}
