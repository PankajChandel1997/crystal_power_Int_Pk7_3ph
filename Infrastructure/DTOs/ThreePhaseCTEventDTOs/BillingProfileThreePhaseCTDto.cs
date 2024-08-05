using System.ComponentModel;

namespace Infrastructure.DTOs.ThreePhaseEventCTDTOs
{
    public class BillingProfileThreePhaseCTDto
    {
        public int Number { get; set; } //For Index Postion
        public string CreatedOn { get; set; }
        public string MeterNo { get; set; }
        public string BillingDate { get; set; }
        public string AveragePFForBillingPeriod { get; set; }
        public string RealTimeClock { get; set; }
        //public string SystemPowerFactorImport { get; set; }
        public string CumulativeEnergykWh { get; set; }
        public string CumulativeEnergykWhTZ1 { get; set; }
        public string CumulativeEnergykWhTZ2 { get; set; }
        public string CumulativeEnergykWhTZ3 { get; set; }
        public string CumulativeEnergykWhTZ4 { get; set; }
        public string CumulativeEnergykWhTZ5 { get; set; }
        public string CumulativeEnergykWhTZ6 { get; set; }
        public string CumulativeEnergykWhTZ7 { get; set; }
        public string CumulativeEnergykWhTZ8 { get; set; }

        public string CumulativeEnergykVAhImport { get; set; }
        public string CumulativeEnergykVAhTZ1 { get; set; }
        public string CumulativeEnergykVAhTZ2 { get; set; }
        public string CumulativeEnergykVAhTZ3 { get; set; }
        public string CumulativeEnergykVAhTZ4 { get; set; }
        public string CumulativeEnergykVAhTZ5 { get; set; }
        public string CumulativeEnergykVAhTZ6 { get; set; }
        public string CumulativeEnergykVAhTZ7 { get; set; }
        public string CumulativeEnergykVAhTZ8 { get; set; }

        [DisplayName("MD kW")]
        public string MaximumDemandkW { get; set; }

        [DisplayName("MD kw DateTime")]
        public string MaximumDemandkWDateTime { get; set; }

        [DisplayName("MD kW TZ1")]
        public string MaximumDemandkWForTZ1 { get; set; }

        [DisplayName("MD kW TZ1 DateTime")]
        public string MaximumDemandkWForTZ1DateTime { get; set; }

        [DisplayName("MD kW TZ2")]
        public string MaximumDemandkWForTZ2 { get; set; }

        [DisplayName("MD kW TZ2 DateTime")]
        public string MaximumDemandkWForTZ2DateTime { get; set; }

        [DisplayName("MD kW TZ3")]
        public string MaximumDemandkWForTZ3 { get; set; }

        [DisplayName("MD kW TZ3 DateTime")]
        public string MaximumDemandkWForTZ3DateTime { get; set; }

        [DisplayName("MD kW TZ4")]
        public string MaximumDemandkWForTZ4 { get; set; }

        [DisplayName("MD kW TZ4 DateTime")]
        public string MaximumDemandkWForTZ4DateTime { get; set; }

        [DisplayName("MD kW TZ5")]
        public string MaximumDemandkWForTZ5 { get; set; }

        [DisplayName("MD kW TZ5 DateTime")]
        public string MaximumDemandkWForTZ5DateTime { get; set; }

        [DisplayName("MD kW TZ6")]
        public string MaximumDemandkWForTZ6 { get; set; }

        [DisplayName("MD kW TZ6 DateTime")]
        public string MaximumDemandkWForTZ6DateTime { get; set; }

        [DisplayName("MD kW TZ7")]
        public string MaximumDemandkWForTZ7 { get; set; }

        [DisplayName("MD kW TZ7 DateTime")]
        public string MaximumDemandkWForTZ7DateTime { get; set; }

        [DisplayName("MD kW TZ8")]
        public string MaximumDemandkWForTZ8 { get; set; }

        [DisplayName("MD kW TZ8 DateTime")]
        public string MaximumDemandkWForTZ8DateTime { get; set; }

        [DisplayName("MD kVA")]
        public string MaximumDemandkVA { get; set; }

        [DisplayName("MD kVA DateTime")]
        public string MaximumDemandkVADateTime { get; set; }

        [DisplayName("MD kVA TZ1")]
        public string MaximumDemandkVAForTZ1 { get; set; }

        [DisplayName("MD kVA TZ1 DateTime")]
        public string MaximumDemandkVAForTZ1DateTime { get; set; }

        [DisplayName("MD kVA TZ2")]
        public string MaximumDemandkVAForTZ2 { get; set; }

        [DisplayName("MD kVA TZ2 DateTime")]
        public string MaximumDemandkVAForTZ2DateTime { get; set; }

        [DisplayName("MD kVA TZ3")]
        public string MaximumDemandkVAForTZ3 { get; set; }

        [DisplayName("MD kVA TZ3 DateTime")]
        public string MaximumDemandkVAForTZ3DateTime { get; set; }

        [DisplayName("MD kVA TZ4")]
        public string MaximumDemandkVAForTZ4 { get; set; }

        [DisplayName("MD kVA TZ4 DateTime")]
        public string MaximumDemandkVAForTZ4DateTime { get; set; }

        [DisplayName("MD kVA TZ5")]
        public string MaximumDemandkVAForTZ5 { get; set; }

        [DisplayName("MD kVA TZ5 DateTime")]
        public string MaximumDemandkVAForTZ5DateTime { get; set; }

        [DisplayName("MD kVA TZ6")]
        public string MaximumDemandkVAForTZ6 { get; set; }

        [DisplayName("MD kVA TZ6 DateTime")]
        public string MaximumDemandkVAForTZ6DateTime { get; set; }

        [DisplayName("MD kVA TZ7")]
        public string MaximumDemandkVAForTZ7 { get; set; }

        [DisplayName("MD kVA TZ7 DateTime")]
        public string MaximumDemandkVAForTZ7DateTime { get; set; }

        [DisplayName("MD kVA TZ8")]

        public string MaximumDemandkVAForTZ8 { get; set; }

        [DisplayName("MD kVA TZ8 DateTime")]
        public string MaximumDemandkVAForTZ8DateTime { get; set; }
        public string BillingPowerONdurationInMinutesDBP { get; set; }
        public string CumulativeEnergykWhExport { get; set; }
        public string CumulativeEnergykVAhExport { get; set; }
        public string CumulativeEnergykVArhQ1 { get; set; }
        public string CumulativeEnergykVArhQ2 { get; set; }
        public string CumulativeEnergykVArhQ3 { get; set; }
        public string CumulativeEnergykVArhQ4 { get; set; }
        //public string TamperCount { get; set; }

        //Consumption
        public string CumulativeEnergykWhImportConsumption { get; set; }
        public string CumulativeEnergykWhExportConsumption { get; set; }
        public string CumulativeEnergykVAhImportConsumption { get; set; }
        public string CumulativeEnergykVAhExportConsumption { get; set; }

    }
}
