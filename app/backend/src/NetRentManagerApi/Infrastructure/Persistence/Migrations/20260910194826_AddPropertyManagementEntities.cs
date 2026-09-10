using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetRentManagerApi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertyManagementEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "properties",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    address = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    bedroom_count = table.Column<int>(type: "integer", nullable: false),
                    bathroom_count = table.Column<int>(type: "integer", nullable: false),
                    area_square_meters = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    image_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_properties", x => x.id);
                    table.CheckConstraint("ck_properties_area_square_meters_positive", "area_square_meters > 0");
                    table.CheckConstraint("ck_properties_bathroom_count_non_negative", "bathroom_count >= 0");
                    table.CheckConstraint("ck_properties_bedroom_count_non_negative", "bedroom_count >= 0");
                    table.CheckConstraint("ck_properties_price_non_negative", "price >= 0");
                });

            migrationBuilder.CreateTable(
                name: "property_statuses",
                columns: table => new
                {
                    value = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_property_statuses", x => x.value);
                });

            migrationBuilder.CreateIndex(
                name: "ix_properties_status",
                table: "properties",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_property_statuses_value",
                table: "property_statuses",
                column: "value",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "properties");

            migrationBuilder.DropTable(
                name: "property_statuses");
        }
    }
}
