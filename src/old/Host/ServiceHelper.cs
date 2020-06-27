namespace Host
{
#pragma warning disable SA1307 // AccessibleFieldsMustBeginWithUpperCaseLetter
#pragma warning disable SA1201 // ElementsMustAppearInTheCorrectOrder
#pragma warning disable SA1310 // FieldNamesMustNotContainUnderscore
#pragma warning disable SA1306 // FieldNamesMustBeginWithLowerCaseLetter

    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    public static class ServiceHelper
    {
        public static bool IsService()
        {
            using (var process = GetParent(Process.GetCurrentProcess()))
            {
                return process != null && process.ProcessName == "services";
            }
        }

        public static Process GetParent(Process child)
        {
            var parentId = 0;

            var handle = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);

            if (handle == IntPtr.Zero)
            {
                return null;
            }

            var processInfo = new PROCESSENTRY32
            {
                dwSize = (uint)Marshal.SizeOf(typeof(PROCESSENTRY32)),
            };

            if (!Process32First(handle, ref processInfo))
            {
                return null;
            }

            do
            {
                if (child.Id == processInfo.th32ProcessID)
                {
                    parentId = (int)processInfo.th32ParentProcessID;
                }
            }
            while (parentId == 0 && Process32Next(handle, ref processInfo));

            if (parentId > 0)
            {
                return Process.GetProcessById(parentId);
            }

            return null;
        }

        private static readonly uint TH32CS_SNAPPROCESS = 2;

        [DllImport("kernel32.dll")]
        public static extern bool Process32Next(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateToolhelp32Snapshot(uint dwFlags, uint th32ProcessID);

        [DllImport("kernel32.dll")]
        public static extern bool Process32First(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESSENTRY32
        {
            public uint dwSize;
            public uint cntUsage;
            public uint th32ProcessID;
            public IntPtr th32DefaultHeapID;
            public uint th32ModuleID;
            public uint cntThreads;
            public uint th32ParentProcessID;
            public int pcPriClassBase;
            public uint dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szExeFile;
        }
    }
}
