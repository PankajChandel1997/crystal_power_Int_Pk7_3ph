namespace Infrastructure.DTOs.EventDTOs
{
    public class ControlEventDto
    {
        public int Number { get; set; }
        public string CreatedOn { get; set; }
        public string MeterNo { get; set; }
        public string RealTimeClockDateAndTime { get; set; }
        public string Event { get; set; }
        public string GenericEventLogSequenceNumber { get; set; }

    }
}
