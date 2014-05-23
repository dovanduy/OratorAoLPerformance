using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace OratorPluginBase
{
    public static class DemanglerService
    {
        [DllImport("demangle.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void GetDemangledCallerByReference(string mangledName, out IntPtr resultPtr);

        [DllImport("demangle.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void FreePointer(IntPtr resultPtr);

        public static string Demangle(string function)
        {
            IntPtr _demangledNamePtr = new IntPtr();
            GetDemangledCallerByReference(function, out _demangledNamePtr);
            if (_demangledNamePtr != IntPtr.Zero)
            {
                function = Marshal.PtrToStringAnsi(_demangledNamePtr);
            }
            DemanglerService.FreePointer(_demangledNamePtr);

            return function;
        }
    }
}
