// This file is subject to the terms and conditions defined
// in file 'LICENSE', which is part of this source code package.

using System;
using System.Threading;

namespace DepotDownloader
{
    internal sealed class JobTelemetry : IDisposable
    {
        private static long downloadedBytes;
        private static long validationBytes;
        private static volatile bool enabled;
        private readonly Timer timer;
        private int writing;

        private JobTelemetry()
        {
            enabled = true;
            Report(null);
            timer = new Timer(Report, null, 1000, 1000);
        }

        public static JobTelemetry Start() =>
            Environment.GetEnvironmentVariable("DEPOTDROP_TELEMETRY") == "1" ? new JobTelemetry() : null;

        public static void Downloaded(long bytes)
        {
            if (enabled) Interlocked.Add(ref downloadedBytes, bytes);
        }

        public static void Validated(long bytes)
        {
            if (enabled && bytes > 0) Interlocked.Add(ref validationBytes, bytes);
        }

        private void Report(object state)
        {
            if (Interlocked.Exchange(ref writing, 1) != 0) return;
            try
            {
                Console.WriteLine("DEPOTDROP_METRICS {\"downloadedBytes\":" + Interlocked.Read(ref downloadedBytes)
                    + ",\"validationBytes\":" + Interlocked.Read(ref validationBytes) + "}");
            }
            finally { Volatile.Write(ref writing, 0); }
        }

        public void Dispose()
        {
            timer.Dispose();
            Report(null);
            enabled = false;
        }
    }
}
