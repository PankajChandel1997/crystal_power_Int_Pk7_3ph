using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interface
{
    public interface ITrackCreated
    {
        string CreatedOn { get; set; }
        int CreatedBy { get; set; }
    }
}
