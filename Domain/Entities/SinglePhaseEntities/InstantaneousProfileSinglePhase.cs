using Domain.Interface;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.SinglePhaseEntities
{
    public class InstantaneousProfileSinglePhase : Entity<int>, ITrackCreated, ITrackUpdated
    {
        public string MeterNo { get; set; }
        public string Realtimeclock { get; set; }
        public string Voltage { get; set; }
        public string PhaseCurrent { get; set; }
        public string NeutralCurrent { get; set; }
        public string SignedPowerFactor { get; set; }
        public string FrequencyHz { get; set; }
        public string ApparentPowerKVA { get; set; }
        public string ActivePowerkW { get; set; }
        public string CumulativeenergykWhimport { get; set; }
        public string CumulativeenergykVAhimport { get; set; }
        public string MaxumumDemandkW { get; set; }
        public string MaxumumDemandkWdateandtime { get; set; }
        public string MaxumumDemandkVA { get; set; }
        public string MaxumumDemandkVAdateandtime { get; set; }
        public string CumulativepowerONdurationinminute { get; set; }
        public string Cumulativetampercount { get; set; }
        public string Cumulativebillingcount { get; set; }
        public string Cumulativeprogrammingcount { get; set; }
        public string CumulativeenergykWhExport { get; set; }
        public string CumulativeenergykVAhExport { get; set; }
        public string Loadlimitfunctionstatus { get; set; }
        public string LoadlimitvalueinkW { get; set; }


        public string CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public string UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
