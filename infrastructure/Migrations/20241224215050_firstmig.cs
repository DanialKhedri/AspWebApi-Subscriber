using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class firstmig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataPoint1Value = table.Column<int>(type: "int", nullable: false),
                    DataPoint2Value = table.Column<int>(type: "int", nullable: false),
                    DataPoint3Value = table.Column<int>(type: "int", nullable: false),
                    DataPoint4Value = table.Column<int>(type: "int", nullable: false),
                    DataPoint5Value = table.Column<int>(type: "int", nullable: false),
                    DataPoint6Value = table.Column<int>(type: "int", nullable: false),
                    DataPoint7Value = table.Column<int>(type: "int", nullable: false),
                    DataPoint8Value = table.Column<int>(type: "int", nullable: false),
                    DataPoint9Value = table.Column<int>(type: "int", nullable: false),
                    DataPoint10Value = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataRecords", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataRecords");
        }
    }
}
