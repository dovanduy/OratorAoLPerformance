using System;

namespace Profiler.LogManager.Models
{
    public interface IShellOutputReceiver
    {
        void AddOutput(byte[] data, int offset, int length);
        void Flush();

        bool IsCancelled { get; }
    }
}

