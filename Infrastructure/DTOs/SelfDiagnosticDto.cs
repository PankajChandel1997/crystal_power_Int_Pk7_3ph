namespace Infrastructure.DTOs
{
    public class SelfDiagnosticDto
    {
        public int Number { get; set; } //For Index Postion
        public string CreatedOn { get; set; }
        public string MeterNo { get; set; }
        public string Status { get; set; }
        public string RTCBattery { get; set; }
        public string MainBattery { get; set; }
    }
}
