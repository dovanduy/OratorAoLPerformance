using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Media;

using ProtoBuf;

namespace OratorPluginBase
{
    [ProtoContract]
    public class SerializableColor
    {
        [ProtoMember(1)]
        private byte _colorA;
        [ProtoMember(2)]
        private byte _colorR;
        [ProtoMember(3)]
        private byte _colorG;
        [ProtoMember(4)]
        private byte _colorB;

        public Color Color
        {
            get
            {
                return Color.FromArgb(_colorA, _colorR, _colorG, _colorB);
            }
            set
            {
                _colorA = value.A;
                _colorR = value.R;
                _colorG = value.G;
                _colorB = value.B;
            }
        }
    }
}
