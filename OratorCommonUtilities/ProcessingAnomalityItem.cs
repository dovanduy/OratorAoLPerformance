using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Media.Imaging;

using ProtoBuf;

namespace OratorCommonUtilities
{
    [ProtoContract]
    public abstract class ProcessingAnomalityItem : ViewModelBase
    {
        [ProtoMember(1)]
        public string Message { get; set; }
        [ProtoMember(2)]
        public int AnomalyCount { get; set; }

        public abstract BitmapImage Icon { get; }    
    }  
}
