using Domain.Interface;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.ThreePhaseCTEntities
{
    public class BillingProfileThreePhaseCT : Entity<int>, ITrackCreated, ITrackUpdated
    {
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
        public string MaximumDemandkW { get; set; }
        public string MaximumDemandkWDateTime { get; set; }
        public string MaximumDemandkWForTZ1 { get; set; }
        public string MaximumDemandkWForTZ1DateTime { get; set; }
        public string MaximumDemandkWForTZ2 { get; set; }
        public string MaximumDemandkWForTZ2DateTime { get; set; }
        public string MaximumDemandkWForTZ3 { get; set; }
        public string MaximumDemandkWForTZ3DateTime { get; set; }
        public string MaximumDemandkWForTZ4 { get; set; }
        public string MaximumDemandkWForTZ4DateTime { get; set; }
        public string MaximumDemandkWForTZ5 { get; set; }
        public string MaximumDemandkWForTZ5DateTime { get; set; }
        public string MaximumDemandkWForTZ6 { get; set; }
        public string MaximumDemandkWForTZ6DateTime { get; set; }
        public string MaximumDemandkWForTZ7 { get; set; }
        public string MaximumDemandkWForTZ7DateTime { get; set; }
        public string MaximumDemandkWForTZ8 { get; set; }
        public string MaximumDemandkWForTZ8DateTime { get; set; }
        public string MaximumDemandkVA { get; set; }
        public string MaximumDemandkVADateTime { get; set; }
        public string MaximumDemandkVAForTZ1 { get; set; }
        public string MaximumDemandkVAForTZ1DateTime { get; set; }
        public string MaximumDemandkVAForTZ2 { get; set; }
        public string MaximumDemandkVAForTZ2DateTime { get; set; }
        public string MaximumDemandkVAForTZ3 { get; set; }
        public string MaximumDemandkVAForTZ3DateTime { get; set; }
        public string MaximumDemandkVAForTZ4 { get; set; }
        public string MaximumDemandkVAForTZ4DateTime { get; set; }
        public string MaximumDemandkVAForTZ5 { get; set; }
        public string MaximumDemandkVAForTZ5DateTime { get; set; }
        public string MaximumDemandkVAForTZ6 { get; set; }
        public string MaximumDemandkVAForTZ6DateTime { get; set; }
        public string MaximumDemandkVAForTZ7 { get; set; }
        public string MaximumDemandkVAForTZ7DateTime { get; set; }
        public string MaximumDemandkVAForTZ8 { get; set; }
        public string MaximumDemandkVAForTZ8DateTime { get; set; }
        public string BillingPowerONdurationInMinutesDBP { get; set; }
        public string CumulativeEnergykWhExport { get; set; }
        public string CumulativeEnergykVAhExport { get; set; }
        public string CumulativeEnergykVArhQ1 { get; set; }
        public string CumulativeEnergykVArhQ2 { get; set; }
        public string CumulativeEnergykVArhQ3 { get; set; }
        public string CumulativeEnergykVArhQ4 { get; set; }
        public string TamperCount { get; set; }

        public string CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public string UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
