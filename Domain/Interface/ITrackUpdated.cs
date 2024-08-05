using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interface
{
    public interface ITrackUpdated
    {
        string UpdatedOn { get; set; }
        int? UpdatedBy { get; set; }
    }
}
