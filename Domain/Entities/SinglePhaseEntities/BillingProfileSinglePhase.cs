using Domain.Interface;
using Domain.Model;

namespace Domain.Entities.SinglePhaseEntities
{
    public class BillingProfileSinglePhase : Entity<int>, ITrackCreated, ITrackUpdated
    {
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

        public string CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public string UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
