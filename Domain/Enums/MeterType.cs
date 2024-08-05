using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    [Flags]
    public enum MeterType : short
    {
        NotAvaliabile = 0,
        Other = 1,
        SinglePhase = 6,
        ThreePhase = 7,
        ThreePhaseLTCT=8,
        ThreePhaseHTCT=10
    }
}
