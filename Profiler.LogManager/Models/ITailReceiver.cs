using System;

namespace Profiler.LogManager.Models
{
    public interface ITailReceiver
    {
        void Tail(byte[] data, int offset, int len);

        bool IsCancelled { get; }
    }
}

