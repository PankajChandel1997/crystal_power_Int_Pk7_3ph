using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs.SinglePhaseEventDTOs
{
    public class DailyLoadProfileSinglePhaseDto
    {
        public int Number { get; set; } //For Index Postion
        public string CreatedOn { get; set; }
        public string MeterNo { get; set; }
        public string RealTimeClock { get; set; }
        public string CumulativeEnergykWhImport { get; set; }
        public string CumulativeEnergyKVAhImport { get; set; }
        public string CumulativeEnergykWhExport { get; set; }
        public string CumulativeEnergykVAhExport { get; set; }
    }
}
