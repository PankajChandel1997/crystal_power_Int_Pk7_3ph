using Domain.Interface;
using Domain.Model;

namespace Domain.Entities
{
    public class OtherEvent : Entity<int>, ITrackCreated, ITrackUpdated
    {
        public string MeterNo { get; set; }
        public string RealTimeClockDateAndTime { get; set; }
        public string EventCode { get; set; }
        public string CurrentIr { get; set; }
        public string CurrentIy { get; set; }
        public string CurrentIb { get; set; }
        public string VoltageVrn { get; set; }
        public string VoltageVyn { get; set; }
        public string VoltageVbn { get; set; }
        public string SignedPowerFactorRPhase { get; set; }
        public string SignedPowerFactorYPhase { get; set; }
        public string SignedPowerFactorBPhase { get; set; }
        public string CumulativeEnergykWhImport { get; set; }
        public string CumulativeTamperCount { get; set; }
        public string CumulativeEnergykWhExport {get; set;}
        public string GenericEventLogSequenceNumber { get; set; }
        public string NuetralCurrent { get; set; }
        public string CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public string UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
