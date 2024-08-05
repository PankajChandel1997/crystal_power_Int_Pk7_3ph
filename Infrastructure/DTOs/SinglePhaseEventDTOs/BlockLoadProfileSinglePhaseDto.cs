using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs.SinglePhaseEventDTOs
{
    public class BlockLoadProfileSinglePhaseDto
    {
        public int Number { get; set; } //For Index Postion
        public string CreatedOn { get; set; }
        public string MeterNo { get; set; }
        public string RealTimeClock { get; set; }
        public string AverageVoltage { get; set; }
        public string BlockEnergykWhImport { get; set; }
        public string BlockEnergykVAh { get; set; }
        public string BlockEnergykWhExport { get; set; }
        public string BlockEnergykVAhExport { get; set; }
        public string PhaseCurrent { get; set; }
        public string NeutralCurrent { get; set; }
    }
}
