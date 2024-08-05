using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatedBilling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AveragePowerFactorExport",
                table: "BillingProfileSinglePhase",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaximumDemandkWExport",
                table: "BillingProfileSinglePhase",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaximumDemandkWExportDateTime",
                table: "BillingProfileSinglePhase",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaximumDemandkvaExport",
                table: "BillingProfileSinglePhase",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaximumDemandkvaExportDateTime",
                table: "BillingProfileSinglePhase",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AveragePowerFactorExport",
                table: "BillingProfileSinglePhase");

            migrationBuilder.DropColumn(
                name: "MaximumDemandkWExport",
                table: "BillingProfileSinglePhase");

            migrationBuilder.DropColumn(
                name: "MaximumDemandkWExportDateTime",
                table: "BillingProfileSinglePhase");

            migrationBuilder.DropColumn(
                name: "MaximumDemandkvaExport",
                table: "BillingProfileSinglePhase");

            migrationBuilder.DropColumn(
                name: "MaximumDemandkvaExportDateTime",
                table: "BillingProfileSinglePhase");
        }
    }
}
