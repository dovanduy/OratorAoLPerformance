using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OratorCommonUtilities.Extensions
{
    public static class StringExtension
    {

        public static byte[] GetBytes(this string req, Encoding enc)
        {
            return enc.GetBytes(req);
        }

        public static byte[] GetBytes(this string req, string encodingName)
        {
            Encoding enc = Encoding.GetEncoding(encodingName);
            return GetBytes(req, enc);
        }
    }
}
