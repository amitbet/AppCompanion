using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;

namespace ProcessExaminator
{
    [Flags]
    public enum DumpOptions : uint
    {
        // From dbghelp.h:
        Normal = 0x00000000,
        WithDataSegs = 0x00000001,
        WithFullMemory = 0x00000002,
        WithHandleData = 0x00000004,
        FilterMemory = 0x00000008,
        ScanMemory = 0x00000010,
        WithUnloadedModules = 0x00000020,
        WithIndirectlyReferencedMemory = 0x00000040,
        FilterModulePaths = 0x00000080,
        WithProcessThreadData = 0x00000100,
        WithPrivateReadWriteMemory = 0x00000200,
        WithoutOptionalData = 0x00000400,
        WithFullMemoryInfo = 0x00000800,
        WithThreadInfo = 0x00001000,
        WithCodeSegs = 0x00002000,
        WithoutAuxiliaryState = 0x00004000,
        WithFullAuxiliaryState = 0x00008000,
        WithPrivateWriteCopyMemory = 0x00010000,
        IgnoreInaccessibleMemory = 0x00020000,
        ValidTypeFlags = 0x0003ffff,
    };


    public class DumpWriter
    {
        //delegate void FilterDelegate();
        //delegate bool FilterDelegate(ref Exception exception);

        //[DllImport("kernel32.dll")]
        //static extern FilterDelegate SetUnhandledExceptionFilter(FilterDelegate lpTopLevelExceptionFilter);

        //static extern bool MiniDumpWriteDump(IntPtr hProcess, uint ProcessId, IntPtr hFile, int DumpType, ref MiniDumpExceptionInformation ExceptionParam, IntPtr UserStreamParam, IntPtr CallbackParam);

        // Overload requiring MiniDumpExceptionInformation
        [DllImport("dbghelp.dll", EntryPoint = "MiniDumpWriteDump", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
        static extern bool MiniDumpWriteDump(IntPtr hProcess, uint processId, SafeHandle hFile, uint dumpType, ref MiniDumpExceptionInformation expParam, IntPtr userStreamParam, IntPtr callbackParam);

        // Overload supporting MiniDumpExceptionInformation == NULL
        [DllImport("dbghelp.dll", EntryPoint = "MiniDumpWriteDump", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
        static extern bool MiniDumpWriteDump(IntPtr hProcess, uint processId, SafeHandle hFile, uint dumpType, IntPtr expParam, IntPtr userStreamParam, IntPtr callbackParam);

        [DllImport("kernel32.dll", EntryPoint = "GetCurrentThreadId", ExactSpelling = true)]
        static extern uint GetCurrentThreadId();

        public static bool WriteDump(int pid, int threadWithExceptionId, string filename,  DumpOptions options, bool hasException)
        {
            using (FileStream dumpFile = File.OpenWrite(filename))
            {
                SafeHandle fileHandle = dumpFile.SafeFileHandle;
                //Process currentProcess = Process.GetCurrentProcess();
                Process currentProcess = Process.GetProcessById(pid);
                IntPtr currentProcessHandle = currentProcess.Handle;
                uint currentProcessId = (uint)currentProcess.Id;

                MiniDumpExceptionInformation exp;
                exp.ThreadId = threadWithExceptionId;//GetCurrentThreadId();
                exp.ClientPointers = false;
                exp.ExceptionPointers = IntPtr.Zero;

                if (hasException)
                {
                    exp.ExceptionPointers = System.Runtime.InteropServices.Marshal.GetExceptionPointers();
                }

                bool bRet = false;
                if (exp.ExceptionPointers == IntPtr.Zero)
                {
                    bRet = MiniDumpWriteDump(currentProcessHandle, currentProcessId, fileHandle, (uint)options, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
                }
                else
                {
                    bRet = MiniDumpWriteDump(currentProcessHandle, currentProcessId, fileHandle, (uint)options, ref exp, IntPtr.Zero, IntPtr.Zero);
                }

                return bRet;
            }
        }



        public static bool WriteDump(int pid, string filename, DumpOptions dumpType)
        {
            return WriteDump(pid, 0, filename, dumpType, false);
        }



        // Taken almost verbatim from http://blog.kalmbach-software.de/2008/12/13/writing-minidumps-in-c/
        [StructLayout(LayoutKind.Sequential, Pack = 4)]  // Pack=4 is important! So it works also for x64!
        public struct MiniDumpExceptionInformation
        {
            public long ThreadId;
            public IntPtr ExceptionPointers;
            [MarshalAs(UnmanagedType.Bool)]
            public bool ClientPointers;
        }

        public struct EXCEPTION_POINTERS
        {
            public EXCEPTION_RECORD ExceptionRecord;
            public IntPtr ContextRecord;
        }
        public struct EXCEPTION_RECORD
        {
            public int ExceptionCode;
            public int ExceptionFlags;
            public IntPtr ExceptionRecord;
            public IntPtr ExceptionAddress;
            public int NumberParameters;
            public IntPtr ExceptionInformation;
        }
        //MiniDumpWriteDump( GetCurrentProcess(), GetCurrentProcessId(), file.SafeFileHandle.DangerousGetHandle(), MiniDumpWithFullMemory, ref info, IntPtr.Zero, IntPtr.Zero );
    }
}
