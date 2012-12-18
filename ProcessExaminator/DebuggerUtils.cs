using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Samples.Debugging.MdbgEngine;
using System.Diagnostics;
using Microsoft.Samples.Debugging.CorDebug;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using System.Configuration;
using Microsoft.Samples.Debugging.Native;

namespace ProcessExaminator
{
    public class DebuggerUtils
    {
        public static string GetTimeString(bool forFile)
        {
            if (forFile)
                return DateTime.Now.ToString("dd~MM~yyyy HH_mm_ss(fff)");
            else
                return DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff");
        }

        public static void HandleException(Exception ex)
        {
            //TODO: write some exception handling code
        }

        private static string m_strOutDir = null;
        public static string GetOutputDirectory()
        {
            if (m_strOutDir == null)
            {
                m_strOutDir = Config.GetStringValue(Config.OutputDir);

                if (string.IsNullOrWhiteSpace(m_strOutDir))
                {
                    m_strOutDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Output");
                }
                if (!Directory.Exists(m_strOutDir))
                    Directory.CreateDirectory(m_strOutDir);
            }
            return m_strOutDir;
        }

        public static string GetExceptionDescFromProcess(MDbgProcess process, bool printInnerExcptions = false)
        {
            object o = process.StopReason;
            ExceptionThrownStopReason m = o as ExceptionThrownStopReason;
            StringBuilder retVal = new StringBuilder();

            string strEventType = "";
            if (m != null)
                strEventType = m.EventType.ToString();
            MDbgThread activeThread = null;

            if (process.Threads.HaveActive && process.Threads.Active.CurrentException.TypeName != "N/A")
            {
                activeThread = process.Threads.Active;
            }
            else
            {
                foreach (MDbgThread t in process.Threads)
                {
                    if (t.CurrentException.TypeName != "N/A")
                    {
                        activeThread = t;
                        break;
                    }
                }
            }

            if (activeThread.CurrentException == null)
                return null;

            retVal.AppendLine(strEventType);
            //collect exception data
            MDbgValue ex = activeThread.CurrentException;
            retVal.Append(GetStringFromException(activeThread, ex));

            if (printInnerExcptions)
            {
                ex = ex.GetField("_innerException");
                for (uint i = 1; !ex.IsNull; ex = ex.GetField("_innerException"), i++)
                {
                    retVal.AppendLine("---InnerException" + i + ": ");
                    retVal.Append(GetStringFromException(null, ex));
                }
            }

            return retVal.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="proc">the process</param>
        /// <param name="threadId">a specific thread to read -1 = all threads</param>
        /// <param name="withArgs">evaluate arguments (more detail)</param>
        /// <returns></returns>
        public static ProcessSample GetCallstacks(MDbgProcess proc, int threadId = -1, bool withArgs = false, bool originFirst = false)
        {
            ProcessSample samples = new ProcessSample();
            MDbgThreadCollection tc = proc.Threads;
            //Console.WriteLine("Attached to pid:{0}", proc.CorProcess.Id);
            foreach (MDbgThread t in tc)
            {
                if (threadId != -1 && t.Id != threadId)
                    continue;

                if (!DebuggerUtils.CheckValidState(t))
                    continue;

                StackSample sample = GetThreadStacks(t, withArgs, originFirst);
                samples.Samples.Add(sample);
            }
            return samples;
        }

        public static StackSample GetThreadStacks(MDbgThread t, bool withArgs = false, bool originFirst = false)
        {
            //Console.WriteLine("Callstack for Thread {0}", t.Id.ToString());
            StackSample sample = new StackSample { ThreadId = t.Id, Time = DateTime.Now };
            MDbgFrame[] frames = new MDbgFrame[0];
            try
            {
                frames = t.Frames.ToArray();
            }
            catch (Exception ex)
            {
                DebuggerUtils.HandleExceptionSilently(ex);
            }

            foreach (MDbgFrame f in frames)
            {
                try
                {
                    string frame = GetFrameString(f, withArgs).Trim();
                    if (string.IsNullOrWhiteSpace(frame))
                        frame = "[Missing frame]";

                    if (originFirst)
                        sample.CallStack.Insert(0, frame);
                    else
                        sample.CallStack.Add(frame);
                }
                catch (Exception ex)
                {
                    DebuggerUtils.HandleExceptionSilently(ex);
                }
            }

            return sample;
        }

        public static string GetFrameString(MDbgFrame frame, bool withArgs = false)
        {
            try
            {
                if (withArgs)
                {
                    if (frame.IsManaged && !frame.Function.FullName.StartsWith("System."))
                        return string.Format("{0}({1})", frame.Function.FullName, string.Join(", ", frame.Function.GetArguments(frame).Select(arg => string.Format("{0} = {1}", arg.Name, arg.GetStringValue(false)))));
                }
                else
                {
                    if (frame.IsInfoOnly)
                        return null;

                    string retVal = frame.Function.FullName + " (";
                    foreach (MDbgValue value2 in frame.Function.GetArguments(frame))
                    {
                        retVal += value2.TypeName + ", ";
                    }
                    retVal = retVal.TrimEnd(", ".ToCharArray()) + ")";
                    return retVal;
                }
                //return frame.Function.FullName;
            }
            catch (Exception ex)
            {
                DebuggerUtils.HandleExceptionSilently(ex);
            }
            return null;
        }
        private static ExceptionInfo GetExceptionInfo(MDbgThread activeThread, MDbgValue ex)
        {
            ExceptionInfo exinfo = new ExceptionInfo();
            exinfo.ExType = ex.TypeName;
            exinfo.Function = (ex.GetField("_exceptionMethodString").IsNull ? null : ex.GetField("_exceptionMethodString").ToString());
            exinfo.Message = (ex.GetField("_message").IsNull ? null : ex.GetField("_message").GetStringValue(false));
            exinfo.Source = (ex.GetField("_source").IsNull ? null : ex.GetField("_source").ToString());

            //StringBuilder sbCallstack = new StringBuilder();
            //try
            //{
            //    if (activeThread != null)
            //        activeThread.Frames.ToList().ForEach(f => sbCallstack.AppendLine(f.ToString()));
            //}
            //catch (Exception ex1)
            //{
            //    DebuggerUtils.HandleExceptionSilently(ex1);
            //}
            exinfo.Callstack = DebuggerUtils.GetThreadStacks(activeThread, true).ToString();
            return exinfo;
        }

        private static string GetStringFromException(MDbgThread activeThread, MDbgValue ex)
        {
            ExceptionInfo exinfo = GetExceptionInfo(activeThread, ex);

            StringBuilder retVal = new StringBuilder();
            string exceptionType = exinfo.ExType;
            if (!string.IsNullOrWhiteSpace(exinfo.Function))
            {
                retVal.AppendLine("at function: " + exinfo.Function);
            }

            if (!string.IsNullOrWhiteSpace(exinfo.Message))
            {
                retVal.AppendLine("Message: " + exinfo.Message);
            }

            if (!string.IsNullOrWhiteSpace(exinfo.Source))
            {
                retVal.AppendLine("in source file: " + exinfo.Source);
            }

            if (!string.IsNullOrWhiteSpace(exinfo.Callstack))
            {
                retVal.AppendLine("Callstack: " + exinfo.Callstack);
            }

            return retVal.ToString();
        }

        private static int m_intCurrentSessionId = Process.GetCurrentProcess().SessionId;
        public static Process GetFirstProcessWithName(string name)
        {
            Process[] processes = Process.GetProcessesByName(name);

            //Process process = processes.Where(p => p.ProcessName.Contains("TelePrompt")).FirstOrDefault();
            Process process = processes.Where(p => p.SessionId == m_intCurrentSessionId).FirstOrDefault();
            if (process == null)
                return null;
            return process;
        }

        /// <summary> From time to time the thread may be in invalid state. </summary>
        public static bool CheckValidState(MDbgThread th)
        {
            try
            {
                th.CorThread.UserState.ToString();
                return true;
            }
            catch (COMException e)
            {
                // The state of the thread is invalid.
                if ((uint)e.ErrorCode == 0x8013132D)
                {
                    return false;
                }
            }
            return true;
        }
        public static MDbgProcess AttachToProcessWithName(string name)
        {
            MDbgEngine debugger = new MDbgEngine();
            MDbgProcess retval = null;

            Process p = DebuggerUtils.GetFirstProcessWithName(name);
            if (p != null)
                retval = DebuggerUtils.AttachToProcess(p.Id, debugger);

            return retval;
        }

        public static MDbgProcess AttachToProcess(int pid, MDbgEngine debugger)
        {
            debugger.Options.SymbolPath = "";
            string ver = null;
            try
            {
                ver = CorDebugger.GetDebuggerVersionFromPid(pid);
            }
            catch (Exception ex)
            {
                //a problem getting the version doesn't mean that we can't attach, try with the default ver
                ver = CorDebugger.GetDefaultDebuggerVersion();
            }

            MDbgProcess proc = debugger.Attach(pid, null, ver);
            DebuggerUtils.DrainAttach(debugger, proc);
            return proc;
        }

        /// <summary>
        /// Skip past fake attach events.
        /// </summary>
        /// <param name="debugger"></param>
        /// <param name="proc"></param>
        public static void DrainAttach(MDbgEngine debugger, MDbgProcess proc)
        {
            bool fOldStatus = debugger.Options.StopOnNewThread;
            debugger.Options.StopOnNewThread = false; // skip while waiting for AttachComplete

            proc.Go().WaitOne();
            Debug.Assert(proc.StopReason is AttachCompleteStopReason);

            debugger.Options.StopOnNewThread = true; // needed for attach= true; // needed for attach

            // Drain the rest of the thread create events.
            while (proc.CorProcess.HasQueuedCallbacks(null))
            {
                proc.Go().WaitOne();
                Debug.Assert(proc.StopReason is ThreadCreatedStopReason);
            }

            debugger.Options.StopOnNewThread = fOldStatus;
        }


        public static void AttachCmd(int pid, MDbgEngine debugger)
        {
            const string versionArg = "ver";
            const string continuationEventArg = "attachEvent";
            const string pidArg = "pid";
            //ArgParser ap = new ArgParser(arguments, versionArg + ":1;" + continuationEventArg + ":1;" + pidArg + ":1");

            //if (ap.Count > 1)
            //{
            //    throw new MDbgShellException("Wrong # of arguments.");
            //}

            //if (!ap.Exists(0) && !ap.OptionPassed(pidArg))
            //{
            //    WriteOutput("Please choose some process to attach");
            //    ProcessEnumCmd("");
            //    return;
            //}

            //int pid;
            //if (ap.Exists(0))
            //{
            //    pid = ap.AsInt(0);
            //    if (ap.OptionPassed(pidArg))
            //    {
            //        //WriteOutput("Do not specify pid option when also passing pid as last argument");
            //        return;
            //    }
            //}
            //else
            //{
            //    Debug.Assert(ap.OptionPassed(pidArg)); // verified above
            //    pid = ap.GetOption(pidArg).AsInt;
            //}


            //
            // Do some sanity checks to give useful end-user errors.
            // 


            // Can't attach to ourselves!
            if (Process.GetCurrentProcess().Id == pid)
            {
                throw new Exception("Cannot attach to myself!");
            }



            // Can't attach to a process that we're already debugging.
            // ICorDebug may enforce this, but the error may not be very descriptive for an end-user.
            // For example, ICD may propogate an error from the OS, and the OS may return
            // something like AccessDenied if another debugger is already attached.
            // This only checks for cases where this same instance of MDbg is already debugging
            // the process of interest. 
            foreach (MDbgProcess procOther in debugger.Processes)
            {
                if (pid == procOther.CorProcess.Id)
                {
                    throw new Exception("Can't attach to process " + pid + " because it's already being debugged");
                }
            }

            // Get the OS handle if there was one
            ///SafeWin32Handle osEventHandle = null;
            ///if (ap.OptionPassed(continuationEventArg))
            ///{
            ///    osEventHandle = new SafeWin32Handle(new IntPtr(ap.GetOption(continuationEventArg).AsHexOrDecInt));
            ///}

            // determine the version to attach to
            string version = MdbgVersionPolicy.GetDefaultAttachVersion(pid);
            //}
            if (version == null)
            {
                throw new Exception("Can't determine what version of the CLR to attach to in process " +
                    pid + ". Use -ver to specify a version");
            }

            // attach
            MDbgProcess p;
            //p = debugger.Attach(pid, osEventHandle, version);
            p = debugger.Attach(pid, null, version);

            p.Go().WaitOne();

            //if (osEventHandle != null)
            //{
            //    osEventHandle.Dispose();
            //}
        }


        public static void DetachCmd(MDbgEngine debugger)
        {
            MDbgProcess active = debugger.Processes.Active;
            active.Breakpoints.DeleteAll();

            active.Detach();

            // We can't wait for targets that never run (e.g. NoninvasiveStopGoController against a dump)
            if (active.CanExecute())
                active.StopEvent.WaitOne();
        }

        internal static void HandleExceptionSilently(Exception ex)
        {
        }
    }
}
