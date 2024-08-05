using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs.ThreePhaseEventCTDTOs
{
    public class DailyLoadProfileThreePhaseCTDto
    {
        public int Number { get; set; } //For Index Postion
        public string CreatedOn { get; set; }
        public string MeterNo { get; set; }
        public string RealTimeClock { get; set; }
        public string CumulativeEnergykWhImport { get; set; }
        public string CumulativeEnergykVAhImport { get; set; }
        public string CumulativeEnergykWhExport { get; set; }
        public string CumulativeEnergykVAhExport { get; set; }

        public string CumulativeEnergykVArhQ1 { get; set; }
        public string CumulativeEnergykVArhQ2 { get; set; }
        public string CumulativeEnergykVArhQ3 { get; set; }
        public string CumulativeEnergykVArhQ4 { get; set; }
    }
}
