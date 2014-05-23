using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OratorCommonUtilities;

using ProtoBuf;

namespace OratorPluginBase
{
    public interface IPluginDataContainer 
    {
        bool Enabled { get; }
        bool Altered { get; set; }
        List<GraphItemBase> GraphItemsList { get; }
    }
}
