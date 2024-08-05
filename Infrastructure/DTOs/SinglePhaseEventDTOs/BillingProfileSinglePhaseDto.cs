using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs.SinglePhaseEventDTOs
{
    public class BillingProfileSinglePhaseDto
    {
        public int Number { get; set; } //For Index Postion
        public string CreatedOn { get; set; }
        public string MeterNo { get; set; }
        public string RealTimeClock { get; set; }
        public string AveragePowerFactor { get; set; }
        public string CumulativeEnergykWhImport { get; set; }
        public string CumulativeEnergykWhTZ1 { get; set; }
        public string CumulativeEnergykWhTZ2 { get; set; }
        public string CumulativeEnergykWhTZ3 { get; set; }
        public string CumulativeEnergykWhTZ4 { get; set; }
        public string CumulativeEnergykWhTZ5 { get; set; }
        public string CumulativeEnergykWhTZ6 { get; set; }
        public string CumulativeEnergykVAhImport { get; set; }
        public string CumulativeEnergykVAhTZ1 { get; set; }
        public string CumulativeEnergykVAhTZ2 { get; set; }
        public string CumulativeEnergykVAhTZ3 { get; set; }
        public string CumulativeEnergykVAhTZ4 { get; set; }
        public string CumulativeEnergykVAhTZ5 { get; set; }
        public string CumulativeEnergykVAhTZ6 { get; set; }
        public string MaximumDemandkW { get; set; }
        public string MaximumDemandkWDateTime { get; set; }
        public string MaximumDemandkVA { get; set; }
        public string MaximumDemandkVADateTime { get; set; }
        public string BillingPowerONdurationinMinutes { get; set; }
        public string CumulativeEnergykWhExport { get; set; }
        public string CumulativeEnergykVAhExport { get; set; }

        public string MaximumDemandkWExport { get; set; }
        public string MaximumDemandkWExportDateTime { get; set; }
        public string MaximumDemandkvaExport { get; set; }
        public string MaximumDemandkvaExportDateTime { get; set; }
        public string AveragePowerFactorExport { get; set; }

        //Consumption
        public string CumulativeEnergykWhImportConsumption { get; set; }
        public string CumulativeEnergykWhExportConsumption { get; set; }
        public string CumulativeEnergykVAhImportConsumption { get; set; }
        public string CumulativeEnergykVAhExportConsumption { get; set; }
    }
}
