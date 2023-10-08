using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserAccessTokens_AccessToken",
                table: "UserAccessTokens");

            migrationBuilder.RenameColumn(
                name: "ExpireDate",
                table: "UserAccessTokens",
                newName: "RefreshTokenExpireDate");

            migrationBuilder.RenameColumn(
                name: "AccessToken",
                table: "UserAccessTokens",
                newName: "RefreshTokenHash");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "AccessTokenExpireDate",
                table: "UserAccessTokens",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccessTokenHash",
                table: "UserAccessTokens",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccessTokens_AccessTokenHash",
                table: "UserAccessTokens",
                column: "AccessTokenHash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserAccessTokens_AccessTokenHash",
                table: "UserAccessTokens");

            migrationBuilder.DropColumn(
                name: "AccessTokenExpireDate",
                table: "UserAccessTokens");

            migrationBuilder.DropColumn(
                name: "AccessTokenHash",
                table: "UserAccessTokens");

            migrationBuilder.RenameColumn(
                name: "RefreshTokenHash",
                table: "UserAccessTokens",
                newName: "AccessToken");

            migrationBuilder.RenameColumn(
                name: "RefreshTokenExpireDate",
                table: "UserAccessTokens",
                newName: "ExpireDate");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccessTokens_AccessToken",
                table: "UserAccessTokens",
                column: "AccessToken",
                unique: true);
        }
    }
}
