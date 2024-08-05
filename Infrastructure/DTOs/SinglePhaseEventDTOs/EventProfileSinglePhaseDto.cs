using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs.SinglePhaseEventDTOs
{
    public class EventProfileSinglePhaseDto
    {
        public int Number { get; set; } //For Index Postion
        public string CreatedOn { get; set; }
        public string MeterNo { get; set; }
        public string DateAndTimeOfEvent { get; set; }
        public string EventCode { get; set; }
        public string Current { get; set; }
        public string Voltage { get; set; }
        public string PowerFactor { get; set; }
        public string CumulativeEnergyKwhImprot { get; set; }
        public string CumulativeEnergyKwhExport { get; set; }
        public string CumulativeTamperCount { get; set; }
    }
}
