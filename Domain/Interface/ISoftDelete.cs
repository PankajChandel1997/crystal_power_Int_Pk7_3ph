using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interface
{
    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
    }
}
