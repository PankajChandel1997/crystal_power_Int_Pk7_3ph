using Domain.Interface;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.SinglePhaseEntities
{
    public class EventProfileSinglePhase : Entity<int>, ITrackCreated, ITrackUpdated
    {
        public string MeterNo { get; set; }
        public string DateAndTimeOfEvent { get; set; }
        public string EventCode { get; set; }
        public string Current { get; set; }
        public string Voltage { get; set; }
        public string PowerFactor { get; set; }
        public string CumulativeEnergyKwhImprot { get; set; }
        public string CumulativeEnergyKwhExport { get; set; }
        public string CumulativeTamperCount { get; set; }

        public string CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public string UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
