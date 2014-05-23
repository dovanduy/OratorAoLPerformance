using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OratorCommonUtilities
{
    public abstract class SessionBase : IEquatable<SessionBase>
    {
        public string SessionName { get; set; }
        public DateTime CreateTimeStamp { get; set; }

        public bool Equals(SessionBase other)
        {
            if (other == null || string.IsNullOrWhiteSpace(other.SessionName))
            { return false; }
            if (!other.GetType().Equals(this.GetType()))
            { return false; }
            return other.SessionName.Equals(this.SessionName);
        }
    }
}
