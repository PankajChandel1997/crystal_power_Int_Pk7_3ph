using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Updatebilling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MaximumDemandkVAExport",
                table: "BillingProfileThreePhase",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaximumDemandkVAExportDateTime",
                table: "BillingProfileThreePhase",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaximumDemandkWExport",
                table: "BillingProfileThreePhase",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaximumDemandkWExportDateTime",
                table: "BillingProfileThreePhase",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PowerFactorExport",
                table: "BillingProfileThreePhase",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaximumDemandkVAExport",
                table: "BillingProfileThreePhase");

            migrationBuilder.DropColumn(
                name: "MaximumDemandkVAExportDateTime",
                table: "BillingProfileThreePhase");

            migrationBuilder.DropColumn(
                name: "MaximumDemandkWExport",
                table: "BillingProfileThreePhase");

            migrationBuilder.DropColumn(
                name: "MaximumDemandkWExportDateTime",
                table: "BillingProfileThreePhase");

            migrationBuilder.DropColumn(
                name: "PowerFactorExport",
                table: "BillingProfileThreePhase");
        }
    }
}
