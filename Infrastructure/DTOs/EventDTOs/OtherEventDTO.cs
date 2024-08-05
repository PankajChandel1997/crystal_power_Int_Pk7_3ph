namespace Infrastructure.DTOs.EventDTOs
{
    public class OtherEventDto
    {
        public int Number { get; set; }
        public string CreatedOn { get; set; }
        public string MeterNo { get; set; }
        public string RealTimeClockDateAndTime { get; set; }
        public string Event { get; set; }
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
        public string CumulativeEnergykWhExport { get; set; }
        public string GenericEventLogSequenceNumber { get; set; }
        //public string NuetralCurrent { get; set; }

    }
}