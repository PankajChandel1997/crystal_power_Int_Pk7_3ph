using Domain.Interface;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.SinglePhaseEntities
{
    public class BlockLoadProfileSinglePhase : Entity<int>, ITrackCreated, ITrackUpdated
    {
        public string MeterNo { get; set; }
        public string RealTimeClock { get; set; }
        public string AverageVoltage { get; set; }
        public string BlockEnergykWhImport { get; set; }
        public string BlockEnergykVAh { get; set; }
        public string BlockEnergykWhExport { get; set; }
        public string BlockEnergykVAhExport { get; set; }
        public string PhaseCurrent { get; set; }
        public string NeutralCurrent { get; set; }

        public string CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public string UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
