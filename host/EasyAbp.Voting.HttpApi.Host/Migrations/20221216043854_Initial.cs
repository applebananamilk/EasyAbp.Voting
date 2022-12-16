using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyAbp.Voting.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VotingActivities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    ActivityName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ActivityStartTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ActivityEndTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Introduction = table.Column<string>(type: "text", nullable: true),
                    BackgroundMusic = table.Column<string>(type: "text", nullable: true),
                    CoverImage = table.Column<string>(type: "text", nullable: true),
                    VotesUnit = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    ContactUs = table.Column<string>(type: "text", nullable: true),
                    IsDraft = table.Column<bool>(type: "boolean", nullable: false),
                    Views = table.Column<long>(type: "bigint", nullable: false),
                    FormContent = table.Column<string>(type: "text", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VotingActivities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VotingPlayers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    ActivityId = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    PlayerNumber = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Avatar = table.Column<string>(type: "text", nullable: true),
                    CoverImage = table.Column<string>(type: "text", nullable: true),
                    FormContent = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    RejectReason = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VotingPlayers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VotingRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    ActivityId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VotingRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VotingBanners",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ActivityId = table.Column<Guid>(type: "uuid", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    Link = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VotingBanners", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VotingBanners_VotingActivities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "VotingActivities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VotingGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ActivityId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VotingGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VotingGroups_VotingActivities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "VotingActivities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VotingPlayerVotes",
                columns: table => new
                {
                    PlayerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Votes = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VotingPlayerVotes", x => x.PlayerId);
                    table.ForeignKey(
                        name: "FK_VotingPlayerVotes_VotingPlayers_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "VotingPlayers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VotingBanners_ActivityId",
                table: "VotingBanners",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_VotingGroups_ActivityId",
                table: "VotingGroups",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_VotingPlayers_ActivityId_GroupId",
                table: "VotingPlayers",
                columns: new[] { "ActivityId", "GroupId" });

            migrationBuilder.CreateIndex(
                name: "IX_VotingRules_ActivityId_Name",
                table: "VotingRules",
                columns: new[] { "ActivityId", "Name" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VotingBanners");

            migrationBuilder.DropTable(
                name: "VotingGroups");

            migrationBuilder.DropTable(
                name: "VotingPlayerVotes");

            migrationBuilder.DropTable(
                name: "VotingRules");

            migrationBuilder.DropTable(
                name: "VotingActivities");

            migrationBuilder.DropTable(
                name: "VotingPlayers");
        }
    }
}
