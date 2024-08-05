using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BillingProfileSinglePhase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    MeterNo = table.Column<string>(type: "TEXT", nullable: true),
                    RealTimeClock = table.Column<string>(type: "TEXT", nullable: true),
                    AveragePowerFactor = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhImport = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhTZ1 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhTZ2 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhTZ3 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhTZ4 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhTZ5 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhTZ6 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVAhImport = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVAhTZ1 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVAhTZ2 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVAhTZ3 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVAhTZ4 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVAhTZ5 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVAhTZ6 = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkW = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkWDateTime = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkVA = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkVADateTime = table.Column<string>(type: "TEXT", nullable: true),
                    BillingPowerONdurationinMinutes = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhExport = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVAhExport = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillingProfileSinglePhase", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BillingProfileThreePhase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    MeterNo = table.Column<string>(type: "TEXT", nullable: true),
                    RealTimeClock = table.Column<string>(type: "TEXT", nullable: true),
                    SystemPowerFactorImport = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWh = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhTZ1 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhTZ2 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhTZ3 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhTZ4 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhTZ5 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhTZ6 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhTZ7 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhTZ8 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVAhImport = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVAhTZ1 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVAhTZ2 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVAhTZ3 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVAhTZ4 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVAhTZ5 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVAhTZ6 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVAhTZ7 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVAhTZ8 = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkW = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkWDateTime = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkWForTZ1 = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkWForTZ1DateTime = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkWForTZ2 = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkWForTZ2DateTime = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkWForTZ3 = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkWForTZ3DateTime = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkWForTZ4 = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkWForTZ4DateTime = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkWForTZ5 = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkWForTZ5DateTime = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkWForTZ6 = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkWForTZ6DateTime = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkWForTZ7 = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkWForTZ7DateTime = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkWForTZ8 = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkWForTZ8DateTime = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkVA = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkVADateTime = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkVAForTZ1 = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkVAForTZ1DateTime = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkVAForTZ2 = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkVAForTZ2DateTime = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkVAForTZ3 = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkVAForTZ3DateTime = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkVAForTZ4 = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkVAForTZ4DateTime = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkVAForTZ5 = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkVAForTZ5DateTime = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkVAForTZ6 = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkVAForTZ6DateTime = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkVAForTZ7 = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkVAForTZ7DateTime = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkVAForTZ8 = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkVAForTZ8DateTime = table.Column<string>(type: "TEXT", nullable: true),
                    BillingPowerONdurationInMinutesDBP = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhExport = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVAhExport = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVArhQ1 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVArhQ2 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVArhQ3 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVArhQ4 = table.Column<string>(type: "TEXT", nullable: true),
                    TamperCount = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillingProfileThreePhase", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BillingProfileThreePhaseCT",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    MeterNo = table.Column<string>(type: "TEXT", nullable: true),
                    BillingDate = table.Column<string>(type: "TEXT", nullable: true),
                    AveragePFForBillingPeriod = table.Column<string>(type: "TEXT", nullable: true),
                    RealTimeClock = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWh = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhTZ1 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhTZ2 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhTZ3 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhTZ4 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhTZ5 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhTZ6 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhTZ7 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhTZ8 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVAhImport = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVAhTZ1 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVAhTZ2 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVAhTZ3 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVAhTZ4 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVAhTZ5 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVAhTZ6 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVAhTZ7 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVAhTZ8 = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkW = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkWDateTime = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkWForTZ1 = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkWForTZ1DateTime = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkWForTZ2 = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkWForTZ2DateTime = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkWForTZ3 = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkWForTZ3DateTime = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkWForTZ4 = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkWForTZ4DateTime = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkWForTZ5 = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkWForTZ5DateTime = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkWForTZ6 = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkWForTZ6DateTime = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkWForTZ7 = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkWForTZ7DateTime = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkWForTZ8 = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkWForTZ8DateTime = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkVA = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkVADateTime = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkVAForTZ1 = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkVAForTZ1DateTime = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkVAForTZ2 = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkVAForTZ2DateTime = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkVAForTZ3 = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkVAForTZ3DateTime = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkVAForTZ4 = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkVAForTZ4DateTime = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkVAForTZ5 = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkVAForTZ5DateTime = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkVAForTZ6 = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkVAForTZ6DateTime = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkVAForTZ7 = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkVAForTZ7DateTime = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkVAForTZ8 = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkVAForTZ8DateTime = table.Column<string>(type: "TEXT", nullable: true),
                    BillingPowerONdurationInMinutesDBP = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhExport = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVAhExport = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVArhQ1 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVArhQ2 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVArhQ3 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVArhQ4 = table.Column<string>(type: "TEXT", nullable: true),
                    TamperCount = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillingProfileThreePhaseCT", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BlockLoadProfileSinglePhase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    MeterNo = table.Column<string>(type: "TEXT", nullable: true),
                    RealTimeClock = table.Column<string>(type: "TEXT", nullable: true),
                    AverageVoltage = table.Column<string>(type: "TEXT", nullable: true),
                    BlockEnergykWhImport = table.Column<string>(type: "TEXT", nullable: true),
                    BlockEnergykVAh = table.Column<string>(type: "TEXT", nullable: true),
                    BlockEnergykWhExport = table.Column<string>(type: "TEXT", nullable: true),
                    BlockEnergykVAhExport = table.Column<string>(type: "TEXT", nullable: true),
                    PhaseCurrent = table.Column<string>(type: "TEXT", nullable: true),
                    NeutralCurrent = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockLoadProfileSinglePhase", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BlockLoadProfileThreePhase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    MeterNo = table.Column<string>(type: "TEXT", nullable: true),
                    RealTimeClock = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentR = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentY = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentB = table.Column<string>(type: "TEXT", nullable: true),
                    VoltageR = table.Column<string>(type: "TEXT", nullable: true),
                    VoltageY = table.Column<string>(type: "TEXT", nullable: true),
                    VoltageB = table.Column<string>(type: "TEXT", nullable: true),
                    PowerFactorRPhase = table.Column<string>(type: "TEXT", nullable: true),
                    PowerFactorYPhase = table.Column<string>(type: "TEXT", nullable: true),
                    PowerFactorBPhase = table.Column<string>(type: "TEXT", nullable: true),
                    BlockEnergykWhImport = table.Column<string>(type: "TEXT", nullable: true),
                    BlockEnergykVAhImport = table.Column<string>(type: "TEXT", nullable: true),
                    BlockEnergykWhExport = table.Column<string>(type: "TEXT", nullable: true),
                    BlockEnergykVAhExport = table.Column<string>(type: "TEXT", nullable: true),
                    MeterHealthIndicator = table.Column<string>(type: "TEXT", nullable: true),
                    ImportAvgPF = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockLoadProfileThreePhase", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BlockLoadProfileThreePhaseCT",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    MeterNo = table.Column<string>(type: "TEXT", nullable: true),
                    RealTimeClock = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentR = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentY = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentB = table.Column<string>(type: "TEXT", nullable: true),
                    VoltageR = table.Column<string>(type: "TEXT", nullable: true),
                    VoltageY = table.Column<string>(type: "TEXT", nullable: true),
                    VoltageB = table.Column<string>(type: "TEXT", nullable: true),
                    BlockEnergykWhImport = table.Column<string>(type: "TEXT", nullable: true),
                    BlockEnergykVAhImport = table.Column<string>(type: "TEXT", nullable: true),
                    BlockEnergykWhExport = table.Column<string>(type: "TEXT", nullable: true),
                    BlockEnergykVAhExport = table.Column<string>(type: "TEXT", nullable: true),
                    BlockEnergykVArhQ1 = table.Column<string>(type: "TEXT", nullable: true),
                    BlockEnergykVArhQ2 = table.Column<string>(type: "TEXT", nullable: true),
                    BlockEnergykVArhQ3 = table.Column<string>(type: "TEXT", nullable: true),
                    BlockEnergykVArhQ4 = table.Column<string>(type: "TEXT", nullable: true),
                    MeterHealthIndicator = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockLoadProfileThreePhaseCT", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ControlEvent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    MeterNo = table.Column<string>(type: "TEXT", nullable: true),
                    RealTimeClockDateAndTime = table.Column<string>(type: "TEXT", nullable: true),
                    EventCode = table.Column<string>(type: "TEXT", nullable: true),
                    GenericEventLogSequenceNumber = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ControlEvent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ControlEventSinglePhase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    MeterNo = table.Column<string>(type: "TEXT", nullable: true),
                    RealTimeClockDateAndTime = table.Column<string>(type: "TEXT", nullable: true),
                    EventCode = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ControlEventSinglePhase", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CurrentRelatedEvent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    MeterNo = table.Column<string>(type: "TEXT", nullable: true),
                    RealTimeClockDateAndTime = table.Column<string>(type: "TEXT", nullable: true),
                    EventCode = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentIr = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentIy = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentIb = table.Column<string>(type: "TEXT", nullable: true),
                    VoltageVrn = table.Column<string>(type: "TEXT", nullable: true),
                    VoltageVyn = table.Column<string>(type: "TEXT", nullable: true),
                    VoltageVbn = table.Column<string>(type: "TEXT", nullable: true),
                    SignedPowerFactorRPhase = table.Column<string>(type: "TEXT", nullable: true),
                    SignedPowerFactorYPhase = table.Column<string>(type: "TEXT", nullable: true),
                    SignedPowerFactorBPhase = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhImport = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeTamperCount = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhExport = table.Column<string>(type: "TEXT", nullable: true),
                    GenericEventLogSequenceNumber = table.Column<string>(type: "TEXT", nullable: true),
                    NuetralCurrent = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrentRelatedEvent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CurrentRelatedEventSinglePhase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    MeterNo = table.Column<string>(type: "TEXT", nullable: true),
                    DateAndTimeOfEvent = table.Column<string>(type: "TEXT", nullable: true),
                    EventCode = table.Column<string>(type: "TEXT", nullable: true),
                    Current = table.Column<string>(type: "TEXT", nullable: true),
                    Voltage = table.Column<string>(type: "TEXT", nullable: true),
                    PowerFactor = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhImport = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhExport = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeTamperCount = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrentRelatedEventSinglePhase", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DailyLoadProfileSinglePhase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    MeterNo = table.Column<string>(type: "TEXT", nullable: true),
                    RealTimeClock = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhImport = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergyKVAhImport = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhExport = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVAhExport = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyLoadProfileSinglePhase", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DailyLoadProfileThreePhase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    MeterNo = table.Column<string>(type: "TEXT", nullable: true),
                    RealTimeClock = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhImport = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVAhImport = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhExport = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVAhExport = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyLoadProfileThreePhase", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DailyLoadProfileThreePhaseCT",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    MeterNo = table.Column<string>(type: "TEXT", nullable: true),
                    RealTimeClock = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhImport = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVAhImport = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhExport = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVAhExport = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVArhQ1 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVArhQ2 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVArhQ3 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVArhQ4 = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyLoadProfileThreePhaseCT", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DIEvent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    MeterNo = table.Column<string>(type: "TEXT", nullable: true),
                    RealTimeClockDateAndTime = table.Column<string>(type: "TEXT", nullable: true),
                    EventCode = table.Column<string>(type: "TEXT", nullable: true),
                    GenericEventLogSequenceNumber = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DIEvent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DIEventSinglePhase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    MeterNo = table.Column<string>(type: "TEXT", nullable: true),
                    RealTimeClockDateAndTime = table.Column<string>(type: "TEXT", nullable: true),
                    EventCode = table.Column<string>(type: "TEXT", nullable: true),
                    GenericEventLogSequenceNumber = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DIEventSinglePhase", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ESWSinglePhase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    MeterNo = table.Column<string>(type: "TEXT", nullable: true),
                    OverVoltage = table.Column<bool>(type: "INTEGER", nullable: false),
                    LowVoltage = table.Column<bool>(type: "INTEGER", nullable: false),
                    OverCurrent = table.Column<bool>(type: "INTEGER", nullable: false),
                    VerylowPF = table.Column<bool>(type: "INTEGER", nullable: false),
                    EarthLoading = table.Column<bool>(type: "INTEGER", nullable: false),
                    InfluenceOfPermanetMagnetOorAcDc = table.Column<bool>(type: "INTEGER", nullable: false),
                    NeutralDisturbance = table.Column<bool>(type: "INTEGER", nullable: false),
                    MeterCoverOpen = table.Column<bool>(type: "INTEGER", nullable: false),
                    MeterLoadDisconnectConnected = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastGasp = table.Column<bool>(type: "INTEGER", nullable: false),
                    FirstBreath = table.Column<bool>(type: "INTEGER", nullable: false),
                    IncrementInBillingCounterMRI = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ESWSinglePhase", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ESWThreePhase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    MeterNo = table.Column<string>(type: "TEXT", nullable: true),
                    RPhaseVoltageMissing = table.Column<bool>(type: "INTEGER", nullable: false),
                    YPhaseVoltageMissing = table.Column<bool>(type: "INTEGER", nullable: false),
                    BPhaseVoltageMissing = table.Column<bool>(type: "INTEGER", nullable: false),
                    OverVoltage = table.Column<bool>(type: "INTEGER", nullable: false),
                    LowVoltage = table.Column<bool>(type: "INTEGER", nullable: false),
                    VoltagUnbalance = table.Column<bool>(type: "INTEGER", nullable: false),
                    RPhaseCurrentReverse = table.Column<bool>(type: "INTEGER", nullable: false),
                    YPhaseCurrentReverse = table.Column<bool>(type: "INTEGER", nullable: false),
                    BPhaseCurrentReverse = table.Column<bool>(type: "INTEGER", nullable: false),
                    CurrentUnbalance = table.Column<bool>(type: "INTEGER", nullable: false),
                    CurrentBypass = table.Column<bool>(type: "INTEGER", nullable: false),
                    OverCurrent = table.Column<bool>(type: "INTEGER", nullable: false),
                    VerylowPF = table.Column<bool>(type: "INTEGER", nullable: false),
                    InfluenceOfPermanetMagnetOorAcDc = table.Column<bool>(type: "INTEGER", nullable: false),
                    NeutralDisturbance = table.Column<bool>(type: "INTEGER", nullable: false),
                    MeterCoverOpen = table.Column<bool>(type: "INTEGER", nullable: false),
                    MeterLoadDisconnectConnected = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastGasp = table.Column<bool>(type: "INTEGER", nullable: false),
                    FirstBreath = table.Column<bool>(type: "INTEGER", nullable: false),
                    IncrementInBillingCounterMRI = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ESWThreePhase", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventProfileSinglePhase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    MeterNo = table.Column<string>(type: "TEXT", nullable: true),
                    DateAndTimeOfEvent = table.Column<string>(type: "TEXT", nullable: true),
                    EventCode = table.Column<string>(type: "TEXT", nullable: true),
                    Current = table.Column<string>(type: "TEXT", nullable: true),
                    Voltage = table.Column<string>(type: "TEXT", nullable: true),
                    PowerFactor = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergyKwhImprot = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergyKwhExport = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeTamperCount = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventProfileSinglePhase", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventProfileThreePhase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    MeterNo = table.Column<string>(type: "TEXT", nullable: true),
                    DateAndTimeOfEvent = table.Column<string>(type: "TEXT", nullable: true),
                    EventCode = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentR = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentY = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentB = table.Column<string>(type: "TEXT", nullable: true),
                    VoltageR = table.Column<string>(type: "TEXT", nullable: true),
                    VoltageY = table.Column<string>(type: "TEXT", nullable: true),
                    VoltageB = table.Column<string>(type: "TEXT", nullable: true),
                    PowerFactorRPhase = table.Column<string>(type: "TEXT", nullable: true),
                    PowerFactorYPhase = table.Column<string>(type: "TEXT", nullable: true),
                    PowerFactorBPhase = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhImport = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhExport = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventProfileThreePhase", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventProfileThreePhaseCT",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    MeterNo = table.Column<string>(type: "TEXT", nullable: true),
                    DateAndTimeOfEvent = table.Column<string>(type: "TEXT", nullable: true),
                    EventCode = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentR = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentY = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentB = table.Column<string>(type: "TEXT", nullable: true),
                    VoltageR = table.Column<string>(type: "TEXT", nullable: true),
                    VoltageY = table.Column<string>(type: "TEXT", nullable: true),
                    VoltageB = table.Column<string>(type: "TEXT", nullable: true),
                    PowerFactorRPhase = table.Column<string>(type: "TEXT", nullable: true),
                    PowerFactorYPhase = table.Column<string>(type: "TEXT", nullable: true),
                    PowerFactorBPhase = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhImport = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhExport = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeTamperCount = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventProfileThreePhaseCT", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InstantaneousProfileSinglePhase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    MeterNo = table.Column<string>(type: "TEXT", nullable: true),
                    Realtimeclock = table.Column<string>(type: "TEXT", nullable: true),
                    Voltage = table.Column<string>(type: "TEXT", nullable: true),
                    PhaseCurrent = table.Column<string>(type: "TEXT", nullable: true),
                    NeutralCurrent = table.Column<string>(type: "TEXT", nullable: true),
                    SignedPowerFactor = table.Column<string>(type: "TEXT", nullable: true),
                    FrequencyHz = table.Column<string>(type: "TEXT", nullable: true),
                    ApparentPowerKVA = table.Column<string>(type: "TEXT", nullable: true),
                    ActivePowerkW = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeenergykWhimport = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeenergykVAhimport = table.Column<string>(type: "TEXT", nullable: true),
                    MaxumumDemandkW = table.Column<string>(type: "TEXT", nullable: true),
                    MaxumumDemandkWdateandtime = table.Column<string>(type: "TEXT", nullable: true),
                    MaxumumDemandkVA = table.Column<string>(type: "TEXT", nullable: true),
                    MaxumumDemandkVAdateandtime = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativepowerONdurationinminute = table.Column<string>(type: "TEXT", nullable: true),
                    Cumulativetampercount = table.Column<string>(type: "TEXT", nullable: true),
                    Cumulativebillingcount = table.Column<string>(type: "TEXT", nullable: true),
                    Cumulativeprogrammingcount = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeenergykWhExport = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeenergykVAhExport = table.Column<string>(type: "TEXT", nullable: true),
                    Loadlimitfunctionstatus = table.Column<string>(type: "TEXT", nullable: true),
                    LoadlimitvalueinkW = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstantaneousProfileSinglePhase", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InstantaneousProfileThreePhase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    MeterNo = table.Column<string>(type: "TEXT", nullable: true),
                    RealTimeClockDateAndTime = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentR = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentY = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentB = table.Column<string>(type: "TEXT", nullable: true),
                    VoltageR = table.Column<string>(type: "TEXT", nullable: true),
                    VoltageY = table.Column<string>(type: "TEXT", nullable: true),
                    VoltageB = table.Column<string>(type: "TEXT", nullable: true),
                    SignedPowerFactorRPhase = table.Column<string>(type: "TEXT", nullable: true),
                    SignedPowerFactorYPhase = table.Column<string>(type: "TEXT", nullable: true),
                    SignedPowerFactorBPhase = table.Column<string>(type: "TEXT", nullable: true),
                    ThreePhasePowerFactoRF = table.Column<string>(type: "TEXT", nullable: true),
                    FrequencyHz = table.Column<string>(type: "TEXT", nullable: true),
                    ApparentPowerKVA = table.Column<string>(type: "TEXT", nullable: true),
                    SignedActivePowerkW = table.Column<string>(type: "TEXT", nullable: true),
                    SignedReactivePowerkvar = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhImport = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhExport = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVAhImport = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVAhExport = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVArhQ1 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVArhQ2 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVArhQ3 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVArhQ4 = table.Column<string>(type: "TEXT", nullable: true),
                    NumberOfPowerFailures = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativePowerOFFDurationInMin = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeTamperCount = table.Column<string>(type: "TEXT", nullable: true),
                    BillingPeriodCounter = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeProgrammingCount = table.Column<string>(type: "TEXT", nullable: true),
                    BillingDateImportMode = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkW = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkWDateTime = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkVA = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkVADateTime = table.Column<string>(type: "TEXT", nullable: true),
                    LoadLimitFunctionStatus = table.Column<string>(type: "TEXT", nullable: true),
                    LoadLimitThresholdkW = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstantaneousProfileThreePhase", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InstantaneousProfileThreePhaseCT",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    MeterNo = table.Column<string>(type: "TEXT", nullable: true),
                    RealTimeClockDateAndTime = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentR = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentY = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentB = table.Column<string>(type: "TEXT", nullable: true),
                    VoltageR = table.Column<string>(type: "TEXT", nullable: true),
                    VoltageY = table.Column<string>(type: "TEXT", nullable: true),
                    VoltageB = table.Column<string>(type: "TEXT", nullable: true),
                    SignedPowerFactorRPhase = table.Column<string>(type: "TEXT", nullable: true),
                    SignedPowerFactorYPhase = table.Column<string>(type: "TEXT", nullable: true),
                    SignedPowerFactorBPhase = table.Column<string>(type: "TEXT", nullable: true),
                    ThreePhasePowerFactorPF = table.Column<string>(type: "TEXT", nullable: true),
                    FrequencyHz = table.Column<string>(type: "TEXT", nullable: true),
                    ApparentPowerKVA = table.Column<string>(type: "TEXT", nullable: true),
                    SignedActivePowerkW = table.Column<string>(type: "TEXT", nullable: true),
                    SignedReactivePowerkvar = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhImport = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhExport = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVAhImport = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVAhExport = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVArhQ1 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVArhQ2 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVArhQ3 = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykVArhQ4 = table.Column<string>(type: "TEXT", nullable: true),
                    NumberOfPowerFailures = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativePowerOFFDurationInMin = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeTamperCount = table.Column<string>(type: "TEXT", nullable: true),
                    BillingPeriodCounter = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeProgrammingCount = table.Column<string>(type: "TEXT", nullable: true),
                    BillingDateImportMode = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkW = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkWDateTime = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkVA = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumDemandkVADateTime = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstantaneousProfileThreePhaseCT", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Meter",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    MeterNo = table.Column<string>(type: "TEXT", nullable: true),
                    DeviceId = table.Column<string>(type: "TEXT", nullable: true),
                    ManufacturerName = table.Column<string>(type: "TEXT", nullable: true),
                    FirmwareVersion = table.Column<string>(type: "TEXT", nullable: true),
                    MeterType = table.Column<short>(type: "INTEGER", nullable: false),
                    Category = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentRating = table.Column<string>(type: "TEXT", nullable: true),
                    MeterYearManufacturer = table.Column<string>(type: "TEXT", nullable: true),
                    CTRatio = table.Column<string>(type: "TEXT", nullable: true),
                    PTRatio = table.Column<string>(type: "TEXT", nullable: true),
                    ManSpecificFirmwareVersion = table.Column<string>(type: "TEXT", nullable: true),
                    ConsumerNo = table.Column<string>(type: "TEXT", nullable: true),
                    ConsumerName = table.Column<string>(type: "TEXT", nullable: true),
                    ConsumerAddress = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meter", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MeterFetchDataLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    MeterNo = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeterFetchDataLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NonRolloverEvent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    MeterNo = table.Column<string>(type: "TEXT", nullable: true),
                    RealTimeClockDateAndTime = table.Column<string>(type: "TEXT", nullable: true),
                    EventCode = table.Column<string>(type: "TEXT", nullable: true),
                    GenericEventLogSequenceNumber = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NonRolloverEvent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NonRolloverEventSinglePhase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    MeterNo = table.Column<string>(type: "TEXT", nullable: true),
                    RealTimeClockDateAndTime = table.Column<string>(type: "TEXT", nullable: true),
                    EventCode = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NonRolloverEventSinglePhase", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OtherEvent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    MeterNo = table.Column<string>(type: "TEXT", nullable: true),
                    RealTimeClockDateAndTime = table.Column<string>(type: "TEXT", nullable: true),
                    EventCode = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentIr = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentIy = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentIb = table.Column<string>(type: "TEXT", nullable: true),
                    VoltageVrn = table.Column<string>(type: "TEXT", nullable: true),
                    VoltageVyn = table.Column<string>(type: "TEXT", nullable: true),
                    VoltageVbn = table.Column<string>(type: "TEXT", nullable: true),
                    SignedPowerFactorRPhase = table.Column<string>(type: "TEXT", nullable: true),
                    SignedPowerFactorYPhase = table.Column<string>(type: "TEXT", nullable: true),
                    SignedPowerFactorBPhase = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhImport = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeTamperCount = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhExport = table.Column<string>(type: "TEXT", nullable: true),
                    GenericEventLogSequenceNumber = table.Column<string>(type: "TEXT", nullable: true),
                    NuetralCurrent = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtherEvent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OtherEventSinglePhase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    MeterNo = table.Column<string>(type: "TEXT", nullable: true),
                    DateandTimeofEvent = table.Column<string>(type: "TEXT", nullable: true),
                    EventCode = table.Column<string>(type: "TEXT", nullable: true),
                    Current = table.Column<string>(type: "TEXT", nullable: true),
                    Voltage = table.Column<string>(type: "TEXT", nullable: true),
                    PowerFactor = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhImport = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhExport = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeTamperCount = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtherEventSinglePhase", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PowerRelatedEvent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    MeterNo = table.Column<string>(type: "TEXT", nullable: true),
                    RealTimeClockDateAndTime = table.Column<string>(type: "TEXT", nullable: true),
                    EventCode = table.Column<string>(type: "TEXT", nullable: true),
                    GenericEventLogSequenceNumber = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PowerRelatedEvent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PowerRelatedEventSinglePhase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    MeterNo = table.Column<string>(type: "TEXT", nullable: true),
                    RealTimeClockDateAndTime = table.Column<string>(type: "TEXT", nullable: true),
                    EventCode = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PowerRelatedEventSinglePhase", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SelfDiagnostic",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    MeterNo = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SelfDiagnostic", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransactionEvent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    MeterNo = table.Column<string>(type: "TEXT", nullable: true),
                    RealTimeClockDateAndTime = table.Column<string>(type: "TEXT", nullable: true),
                    EventCode = table.Column<string>(type: "TEXT", nullable: true),
                    GenericEventLogSequenceNumber = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionEvent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransactionEventSinglePhase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    MeterNo = table.Column<string>(type: "TEXT", nullable: true),
                    RealTimeClockDateAndTime = table.Column<string>(type: "TEXT", nullable: true),
                    EventCode = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionEventSinglePhase", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    Password = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VoltageRelatedEvent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    MeterNo = table.Column<string>(type: "TEXT", nullable: true),
                    RealTimeClockDateAndTime = table.Column<string>(type: "TEXT", nullable: true),
                    EventCode = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentIr = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentIy = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentIb = table.Column<string>(type: "TEXT", nullable: true),
                    VoltageVrn = table.Column<string>(type: "TEXT", nullable: true),
                    VoltageVyn = table.Column<string>(type: "TEXT", nullable: true),
                    VoltageVbn = table.Column<string>(type: "TEXT", nullable: true),
                    SignedPowerFactorRPhase = table.Column<string>(type: "TEXT", nullable: true),
                    SignedPowerFactorYPhase = table.Column<string>(type: "TEXT", nullable: true),
                    SignedPowerFactorBPhase = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhImport = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeTamperCount = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhExport = table.Column<string>(type: "TEXT", nullable: true),
                    GenericEventLogSequenceNumber = table.Column<string>(type: "TEXT", nullable: true),
                    NuetralCurrent = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoltageRelatedEvent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VoltageRelatedEventSinglePhase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    MeterNo = table.Column<string>(type: "TEXT", nullable: true),
                    DateandTimeofEvent = table.Column<string>(type: "TEXT", nullable: true),
                    EventCode = table.Column<string>(type: "TEXT", nullable: true),
                    Current = table.Column<string>(type: "TEXT", nullable: true),
                    Voltage = table.Column<string>(type: "TEXT", nullable: true),
                    PowerFactor = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhImport = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeEnergykWhExport = table.Column<string>(type: "TEXT", nullable: true),
                    CumulativeTamperCount = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedOn = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoltageRelatedEventSinglePhase", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BillingProfileSinglePhase");

            migrationBuilder.DropTable(
                name: "BillingProfileThreePhase");

            migrationBuilder.DropTable(
                name: "BillingProfileThreePhaseCT");

            migrationBuilder.DropTable(
                name: "BlockLoadProfileSinglePhase");

            migrationBuilder.DropTable(
                name: "BlockLoadProfileThreePhase");

            migrationBuilder.DropTable(
                name: "BlockLoadProfileThreePhaseCT");

            migrationBuilder.DropTable(
                name: "ControlEvent");

            migrationBuilder.DropTable(
                name: "ControlEventSinglePhase");

            migrationBuilder.DropTable(
                name: "CurrentRelatedEvent");

            migrationBuilder.DropTable(
                name: "CurrentRelatedEventSinglePhase");

            migrationBuilder.DropTable(
                name: "DailyLoadProfileSinglePhase");

            migrationBuilder.DropTable(
                name: "DailyLoadProfileThreePhase");

            migrationBuilder.DropTable(
                name: "DailyLoadProfileThreePhaseCT");

            migrationBuilder.DropTable(
                name: "DIEvent");

            migrationBuilder.DropTable(
                name: "DIEventSinglePhase");

            migrationBuilder.DropTable(
                name: "ESWSinglePhase");

            migrationBuilder.DropTable(
                name: "ESWThreePhase");

            migrationBuilder.DropTable(
                name: "EventProfileSinglePhase");

            migrationBuilder.DropTable(
                name: "EventProfileThreePhase");

            migrationBuilder.DropTable(
                name: "EventProfileThreePhaseCT");

            migrationBuilder.DropTable(
                name: "InstantaneousProfileSinglePhase");

            migrationBuilder.DropTable(
                name: "InstantaneousProfileThreePhase");

            migrationBuilder.DropTable(
                name: "InstantaneousProfileThreePhaseCT");

            migrationBuilder.DropTable(
                name: "Meter");

            migrationBuilder.DropTable(
                name: "MeterFetchDataLog");

            migrationBuilder.DropTable(
                name: "NonRolloverEvent");

            migrationBuilder.DropTable(
                name: "NonRolloverEventSinglePhase");

            migrationBuilder.DropTable(
                name: "OtherEvent");

            migrationBuilder.DropTable(
                name: "OtherEventSinglePhase");

            migrationBuilder.DropTable(
                name: "PowerRelatedEvent");

            migrationBuilder.DropTable(
                name: "PowerRelatedEventSinglePhase");

            migrationBuilder.DropTable(
                name: "SelfDiagnostic");

            migrationBuilder.DropTable(
                name: "TransactionEvent");

            migrationBuilder.DropTable(
                name: "TransactionEventSinglePhase");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "VoltageRelatedEvent");

            migrationBuilder.DropTable(
                name: "VoltageRelatedEventSinglePhase");
        }
    }
}
