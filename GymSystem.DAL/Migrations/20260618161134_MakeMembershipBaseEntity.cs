using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymSystem.DAL.Migrations
{
    /// <inheritdoc />
    public partial class MakeMembershipBaseEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Membership",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Membership");
        }
    }
}
