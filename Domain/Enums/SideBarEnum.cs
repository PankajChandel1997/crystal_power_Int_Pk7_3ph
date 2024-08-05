using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum MeterDataValueType
    {
        [Description("All")]
        All,

        [Description("IP")]
        IP,

        [Description("Billing Profile")]
        BillingProfile,

        [Description("Block Load Profile")]
        BlockLoadProfile,

        [Description("Daily Load Profile")]
        DailyLoadProfile,

        [Description("Event Only")]
        EventOnly,

        [Description("All Without Load Profile")]
        AllWithoutLoadProfile,

        [Description("Self Diagnostic")]
        SelfDiagnostic,

        [Description("NamePlate")]
        NamePlate,
    }
}
