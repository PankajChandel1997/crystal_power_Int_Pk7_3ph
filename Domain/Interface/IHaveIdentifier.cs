using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interface
{
    public interface IHaveIdentifier<TKey>
    {
        TKey Id { get; set; }
    }
}
