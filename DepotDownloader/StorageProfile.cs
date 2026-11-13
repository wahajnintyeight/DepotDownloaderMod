// This file is subject to the terms and conditions defined
// in file 'LICENSE', which is part of this source code package.

using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace DepotDownloader
{
    internal static class StorageProfile
    {
        private const uint IoctlStorageQueryProperty = 0x2D1400;

        [DllImport("kernel32.dll", EntryPoint = "CreateFileW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern SafeFileHandle CreateFile(string path, uint access, uint share, IntPtr security, uint creation, uint flags, IntPtr template);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeviceIoControl(SafeFileHandle device, uint code, byte[] input, int inputSize, byte[] output, int outputSize, out uint returned, IntPtr overlapped);

        public static bool? HasSeekPenalty(string directory)
        {
            if (!OperatingSystem.IsWindows()) return null;
            var root = Path.GetPathRoot(Path.GetFullPath(directory));
            if (root == null || root.Length != 3 || root[1] != ':') return null;

            using var volume = CreateFile(@"\\.\" + root.Substring(0, 2), 0, 3, IntPtr.Zero, 3, 0, IntPtr.Zero);
            if (volume.IsInvalid) return null;
            var query = new byte[12];
            query[0] = 7; // StorageDeviceSeekPenaltyProperty
            var result = new byte[12];
            if (!DeviceIoControl(volume, IoctlStorageQueryProperty, query, query.Length, result, result.Length, out var returned, IntPtr.Zero) || returned < 9)
                return null;
            return result[8] != 0;
        }
    }
}
