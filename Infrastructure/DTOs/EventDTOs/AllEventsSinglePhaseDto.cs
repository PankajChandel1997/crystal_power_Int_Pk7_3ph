namespace Infrastructure.DTOs.EventDTOs
{
    public class AllEventsSinglePhaseDto
    {
        public int Sno { get; set; }
        public string CreatedOn { get; set; }
        public string MeterNo { get; set; }
        public string RealTimeClock { get; set; }
        public string Event { get; set; }
        public string Current { get; set; }
        public string Voltage { get; set; }
        public string PowerFactor { get; set; }
        public string CumulativeEnergykWhImport { get; set; }
        public string CumulativeEnergykWhExport { get; set; }
        public string CumulativeTamperCount { get; set; }
        public string PowerFailureTime { get; set; }

    }
}
