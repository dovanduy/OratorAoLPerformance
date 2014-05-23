using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OratorCommonUtilities.Extensions
{
    public static class ByteExtension
    {
        public static string GetString(this byte[] buffer, Encoding enc)
        {
            return enc.GetString(buffer);
        }
        public static string GetString(this byte [] buffer, string encodingName)
        {
            Encoding enc = Encoding.GetEncoding(encodingName);
            return GetString(buffer,enc);
        }
    }
}
