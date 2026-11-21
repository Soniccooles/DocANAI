using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocANAI.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameRefreshTokenColumnsToSnakeCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_refresh_token_user_UserId",
                table: "refresh_token");

            migrationBuilder.RenameColumn(
                name: "Token",
                table: "refresh_token",
                newName: "token");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "refresh_token",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "refresh_token",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "refresh_token",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "RevokedByIp",
                table: "refresh_token",
                newName: "revoked_by_ip");

            migrationBuilder.RenameColumn(
                name: "RevokedAt",
                table: "refresh_token",
                newName: "revoked_at");

            migrationBuilder.RenameColumn(
                name: "IsRevoked",
                table: "refresh_token",
                newName: "is_revoked");

            migrationBuilder.RenameColumn(
                name: "ExpiresAt",
                table: "refresh_token",
                newName: "expires_at");

            migrationBuilder.RenameColumn(
                name: "CreatedByIp",
                table: "refresh_token",
                newName: "created_by_ip");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "refresh_token",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_refresh_token_Token",
                table: "refresh_token",
                newName: "IX_refresh_token_token");

            migrationBuilder.RenameIndex(
                name: "IX_refresh_token_UserId",
                table: "refresh_token",
                newName: "IX_refresh_token_user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_refresh_token_user_user_id",
                table: "refresh_token",
                column: "user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_refresh_token_user_user_id",
                table: "refresh_token");

            migrationBuilder.RenameColumn(
                name: "token",
                table: "refresh_token",
                newName: "Token");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "refresh_token",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "refresh_token",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "refresh_token",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "revoked_by_ip",
                table: "refresh_token",
                newName: "RevokedByIp");

            migrationBuilder.RenameColumn(
                name: "revoked_at",
                table: "refresh_token",
                newName: "RevokedAt");

            migrationBuilder.RenameColumn(
                name: "is_revoked",
                table: "refresh_token",
                newName: "IsRevoked");

            migrationBuilder.RenameColumn(
                name: "expires_at",
                table: "refresh_token",
                newName: "ExpiresAt");

            migrationBuilder.RenameColumn(
                name: "created_by_ip",
                table: "refresh_token",
                newName: "CreatedByIp");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "refresh_token",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_refresh_token_token",
                table: "refresh_token",
                newName: "IX_refresh_token_Token");

            migrationBuilder.RenameIndex(
                name: "IX_refresh_token_user_id",
                table: "refresh_token",
                newName: "IX_refresh_token_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_refresh_token_user_UserId",
                table: "refresh_token",
                column: "UserId",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
