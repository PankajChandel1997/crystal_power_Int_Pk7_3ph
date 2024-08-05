using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs.ThreePhaseEventDTOs
{
    public class BlockLoadProfileThreePhaseDto
    {
        public int Number { get; set; } //For Index Postion
        public string CreatedOn { get; set; }
        public string MeterNo { get; set; }
        public string RealTimeClock { get; set; }
        public string CurrentR { get; set; }
        public string CurrentY { get; set; }
        public string CurrentB { get; set; }
        public string VoltageR { get; set; }
        public string VoltageY { get; set; }
        public string VoltageB { get; set; }
        public string PowerFactorRPhase { get; set; }
        public string PowerFactorYPhase { get; set; }
        public string PowerFactorBPhase { get; set; }
        public string BlockEnergykWhImport { get; set; }
        public string BlockEnergykVAhImport { get; set; }
        public string BlockEnergykWhExport { get; set; }
        public string BlockEnergykVAhExport { get; set; }
        public string MeterHealthIndicator { get; set; }
        public string ImportAvgPF { get; set; }
    }
}
