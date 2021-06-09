using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Access_Management.Infrastructure.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuthorizationPolicies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AccessSource = table.Column<int>(type: "int", nullable: false),
                    ApplicationDomain = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorizationPolicies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuthorizationRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PolicyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Route = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorizationRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthorizationRules_AuthorizationPolicies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "AuthorizationPolicies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthorizationRules_PolicyId",
                table: "AuthorizationRules",
                column: "PolicyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthorizationRules");

            migrationBuilder.DropTable(
                name: "AuthorizationPolicies");
        }
    }
}
