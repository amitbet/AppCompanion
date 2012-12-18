using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Samples.Debugging.MdbgEngine;
using Microsoft.Samples.Debugging.CorDebug;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Configuration;
using Microsoft.Samples.Tools.Mdbg;
using System.Collections;

namespace ProcessExaminator
{
    public class ProcessListener
    {
        private bool m_blnIsListening = false;
        private string m_strLastDumpFileName = null;
        private string m_strLastLogFileName = null;
        private MDbgEngine m_debugger = null;
        private string m_strDumpFileDirectory = DebuggerUtils.GetOutputDirectory();
        private string m_strLogFileDirectory = DebuggerUtils.GetOutputDirectory();
        private bool m_blnWriteDumpOnUncaughtExceptions = false;
        private bool m_blnLogExceptions = false;
        private PostCallbackEventHandler m_processEventHandler = null;
        private MDbgProcess m_process = null;
        private int m_processId = -1;

        public int ProcessId
        {
            get { return m_processId; }
            set { m_processId = value; }
        }
        private int m_intNumSamplesInPerf = 20;
        private StringBuilder m_sbProcessLog = new StringBuilder();
        private List<ProcessSample> m_colCallstackSample = null;
        private Dictionary<int, StackTreeNode> m_colPerfSample = null;

        public bool IsListening
        {
            get { return m_blnIsListening; }
            set { m_blnIsListening = value; }
        }


        public string LastLogFileName
        {
            get
            {
                if (m_strLastLogFileName == null)
                {
                    string timeString1 = DebuggerUtils.GetTimeString(true);
                    m_strLastLogFileName = Path.Combine(LogFileDirectory, m_process.Name + timeString1 + ".log");
                }
                return m_strLastLogFileName;
            }
            set { m_strLastLogFileName = value; }
        }

        public string LogFileDirectory
        {
            get { return m_strLogFileDirectory; }
            set { m_strLogFileDirectory = value; }
        }

        public bool LogExceptions
        {
            get { return m_blnLogExceptions; }
            set
            {
                //if (!m_blnIsListening)
                //    throw new Exception("call StartListening first to attach");

                m_blnLogExceptions = value;
                //open a new LogFile
                if (m_blnLogExceptions)
                {
                    string timeString1 = DebuggerUtils.GetTimeString(true);
                    LastLogFileName = Path.Combine(LogFileDirectory, "ProcessListener" + timeString1 + ".log");
                }
            }
        }

        public bool WriteDumpOnUncaughtExceptions
        {
            get { return m_blnWriteDumpOnUncaughtExceptions; }
            set
            {
                //if (!m_blnIsListening)
                //    throw new Exception("call StartListening first to attach");

                m_blnWriteDumpOnUncaughtExceptions = value;
            }
        }

        public string DumpFileDirectory
        {
            get { return m_strDumpFileDirectory; }
            set { m_strDumpFileDirectory = value; }
        }

        public string LastDumpFileName
        {
            get { return m_strLastDumpFileName; }

        }

        public ProcessListener(int pid)
        {
            int intConfNumSamples = Config.GetIntValue(Config.PerfNumSamples);
            if (intConfNumSamples > 0)
                m_intNumSamplesInPerf = intConfNumSamples;
            m_processId = pid;
        }

        /// <summary>
        /// 
        /// attach and detach first to make sure that the process can be attached.
        /// next attach will be fast since the assembly loading is done on the first attach.
        /// </summary>
        /// <param name="pid"></param>
        public Task InitialAttach()
        {
            Task t = Task.Factory.StartNew(() =>
            {
                MDbgEngine debugger = new MDbgEngine();
                MDbgProcess process = DebuggerUtils.AttachToProcess(m_processId, debugger);
                process.CorProcess.Stop(0);
                process.Detach();
            });
            return t;
        }

        public int NumSamplesInPerf
        {
            get { return m_intNumSamplesInPerf; }
            set { m_intNumSamplesInPerf = value; }
        }

        public string ProcessLog
        {
            get { return m_sbProcessLog.ToString(); }
        }

        public void StartListening()
        {
            if (m_processId == -1)
                return;

            if (m_blnIsListening)
                return;

            m_debugger = new MDbgEngine();
            m_process = null;
            m_blnIsListening = true;
            Task t = Task.Factory.StartNew(() =>
            {
                try
                {

                    string timeString1 = DebuggerUtils.GetTimeString(true);
                    m_process = DebuggerUtils.AttachToProcess(m_processId, m_debugger);
                    LastLogFileName = Path.Combine(LogFileDirectory, m_process.Name + "(" + m_processId + ") T" + timeString1 + ".log");
                }
                catch (Exception ex)
                {
                    DebuggerUtils.HandleException(ex);
                }

                try
                {
                    //m_process.Go();
                    //m_process.Go().WaitOne();
                    //m_process.Detach();
                    //m_debugger.Options.StopOnLogMessage = true;
                    m_debugger.Options.StopOnException = true;
                    //m_debugger.Options.StopOnExceptionEnhanced = false;
                    m_debugger.Options.StopOnUnhandledException = true;
                    m_blnIsListening = true;
                    while (m_process.IsAlive)
                    {

                        //if (e.CallbackType == ManagedCallbackType.OnException2)
                        //{
                        //    var ce = (CorException2EventArgs)e.CallbackArgs;

                        //    if (ce.EventType == CorDebugExceptionCallbackType.DEBUG_EXCEPTION_FIRST_CHANCE)
                        //    {
                        //        var thread = process.Threads.Lookup(ce.Thread);

                        //        foreach (var frame in thread.Frames)
                        //        {
                        //            if (!frame.IsManaged || frame.Function.FullName.StartsWith("System."))
                        //                break;

                        //            log.Log("{0}({1})", frame.Function.FullName, string.Join(", ", frame.Function.GetArguments(frame).Select(
                        //                arg => string.Format("{0} = {1}", arg.Name, arg.GetStringValue(false)))));
                        //        }
                        //    }
                        //}

                        try
                        {
                            m_process.Go().WaitOne();
                            //if (e.CallbackType == ManagedCallbackType.OnBreakpoint)
                            //m_process.Go().WaitOne();
                            object o = m_process.StopReason;
                            ExceptionThrownStopReason stopReason = o as ExceptionThrownStopReason;
                            if (stopReason != null)
                            {
                                if (stopReason.EventType == Microsoft.Samples.Debugging.CorDebug.NativeApi.CorDebugExceptionCallbackType.DEBUG_EXCEPTION_UNHANDLED)
                                {
                                    string timeString = DebuggerUtils.GetTimeString(true);

                                    if (WriteDumpOnUncaughtExceptions)
                                    {
                                        m_strLastDumpFileName = Path.Combine(m_strDumpFileDirectory, m_process.Name + timeString + ".dmp");
                                        DumpWriter.WriteDump(m_process.CorProcess.Id, m_strLastDumpFileName, DumpOptions.WithFullMemory);
                                    }
                                }

                                if (LogExceptions)
                                {
                                    //File.AppendAllText(LastLogFileName, DateTime.Now.ToString("dd\\MM\\yyyy mm:hh:ss.fff]") + DebuggerUtils.GetExceptionDescFromProcess(m_process) + "\n");
                                    try
                                    {
                                        string exceptionDesc = DebuggerUtils.GetExceptionDescFromProcess(m_process, true);
                                        //Console.Out.WriteLine("----------------------------------------------------");
                                        //Console.Out.WriteLine(exceptionDesc);
                                        File.AppendAllText(LastLogFileName, "----------------------------------------------------\n" + DateTime.Now.ToString("dd\\MM\\yyyy mm:hh:ss.fff]") + exceptionDesc + "\n");
                                        m_sbProcessLog.AppendLine("----------------------------------------------------\n");
                                        m_sbProcessLog.AppendLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"));
                                        m_sbProcessLog.AppendLine(exceptionDesc);
                                    }
                                    catch (Exception ex)
                                    {
                                        DebuggerUtils.HandleException(ex);
                                    }
                                    //m_process.Go();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            DebuggerUtils.HandleException(ex);
                        }
                    }
                    m_blnIsListening = false;
                }
                catch (Exception ex)
                {
                    DebuggerUtils.HandleException(ex);
                }
                //m_process.PostDebugEvent += m_processEventHandler;
            });

        }



        public void SavePerformanceSampleToBin(Dictionary<int, StackTreeNode> sample, string strFilename)
        {
            BinaryFormatter f = new BinaryFormatter();
            //XmlSerializer ser = new XmlSerializer(typeof(Dictionary<int, StackTreeNode>));

            using (Stream s = File.OpenWrite(strFilename))
            {
                f.Serialize(s, sample);
            }
        }

        /// <summary>
        /// performs detach so process doesn't die when exiting
        /// </summary>
        public void CleanUp()
        {
            Task t = Task.Factory.StartNew(() =>
                 {
                     if (m_process != null && m_process.IsAlive)
                     {

                         m_process.CorProcess.Stop(0);
                         m_process.Detach();

                     }
                 });
            t.Wait();
        }

        //ManualResetEvent m_mreWaitForSample = new ManualResetEvent(false);
        /// <summary>
        /// takes several callstack samples from the process over time and creates a call tree, where performance issues can be evaluated.
        /// </summary>
        /// <param name="onlyMainThread"></param>
        /// <returns>a dictionary of [threadId,StackTreeRoot]</returns>
        public Dictionary<int, StackTreeNode> GetPerformanceSample(bool onlyMainThread = false)
        {
            ProcessSampler sampler = new ProcessSampler(onlyMainThread);
            if (m_blnIsListening)
            {
                throw new Exception("can't take samples while listning!");
                //if (m_process.IsRunning)
                //{
                //    m_process.AsyncStop().WaitOne();
                //    m_process.CorProcess.Stop(0);
                //}
                //List<ProcessSample> sample = sampler.GetSample(m_process, 20, 200);
                //return sampler.CreateStackTrees(sample);

            }
            else
            {
                int timeBetweenSamples = Config.GetIntValue(Config.PerfTimeBetweenSamples);
                if (timeBetweenSamples == int.MinValue)
                    timeBetweenSamples = 400;
                List<ProcessSample> sample = sampler.GetSample(m_processId, m_intNumSamplesInPerf, timeBetweenSamples, false, true);
                return sampler.CreateStackTrees(sample);
            }
        }

        public static Dictionary<string, string> GetLocalVars(MDbgEngine pDebugger, List<string> specificVarNames = null, bool debuggerVarsOpt = false, bool noFuncevalOpt = false, int? expandDepthOpt = null)
        {
            Dictionary<string, string> retVal = new Dictionary<string, string>();
            //const string debuggerVarsOpt = "d";
            //const string noFuncevalOpt = "nf";
            //const string expandDepthOpt = "r";


            //ArgParser ap = new ArgParser(arguments, debuggerVarsOpt + ";" + noFuncevalOpt + ";" + expandDepthOpt + ":1");
            bool canDoFunceval = !noFuncevalOpt;

            int? expandDepth = null;			    // we use optional here because
            // different codes bellow has different
            // default values.
            if (expandDepthOpt != null)
            {
                expandDepth = (int)expandDepthOpt;
                if (expandDepth < 0)
                    throw new MDbgShellException("Depth cannot be negative.");
            }

            MDbgFrame frame = pDebugger.Processes.Active.Threads.Active.CurrentFrame;
            if (debuggerVarsOpt)
            {
                // let's print all debugger variables
                MDbgProcess p = pDebugger.Processes.Active;
                foreach (MDbgDebuggerVar dv in p.DebuggerVars)
                {
                    MDbgValue v = new MDbgValue(p, dv.CorValue);
                    string value = v.GetStringValue(expandDepth == null ? 0 : (int)expandDepth, canDoFunceval);
                    retVal.Add(dv.Name, value);
                }
            }
            else
            {
                //!debuggerVarsOpt && !noFuncevalOpt && expandDepthOpt == null &&
                if (specificVarNames == null || specificVarNames.Count == 0)
                {
                    // get all active variables
                    MDbgFunction f = frame.Function;

                    ArrayList vars = new ArrayList();
                    MDbgValue[] vals = f.GetActiveLocalVars(frame);
                    if (vals != null)
                    {
                        vars.AddRange(vals);
                    }

                    vals = f.GetArguments(frame);
                    if (vals != null)
                    {
                        vars.AddRange(vals);
                    }
                    foreach (MDbgValue v in vars)
                    {
                        string value = v.GetStringValue(expandDepth == null ? 0 : (int)expandDepth, canDoFunceval);
                        retVal.Add(v.Name, value);
                    }
                }
                else
                {
                    // user requested printing of specific variables
                    for (int j = 0; j < specificVarNames.Count; ++j)
                    {
                        MDbgValue var = pDebugger.Processes.Active.ResolveVariable(specificVarNames[j], frame);
                        if (var != null)
                        {
                            string value = var.GetStringValue(expandDepth == null ? 1 : (int)expandDepth, canDoFunceval);
                            retVal.Add(specificVarNames[j], value);
                        }
                        else
                        {
                            throw new MDbgShellException("Variable not found");
                        }
                    }
                }
            }
            return retVal;
        }


        /// <summary>
        /// takes a single callstack snapshot from the process.
        /// </summary>
        /// <param name="onlyMainThread"></param>
        /// <returns></returns>
        public List<ProcessSample> GetCallStacks(bool onlyMainThread = false, bool extraDetails = false)
        {
            List<ProcessSample> sample;
            ProcessSampler sampler = new ProcessSampler(onlyMainThread);
            if (m_blnIsListening)
            {
                throw new Exception("can't take samples while listning!");
                //if (m_process.IsRunning)
                //{
                //    m_process.AsyncStop().WaitOne();
                //    m_process.CorProcess.Stop(0);
                //}

                //sample = sampler.GetSample(m_process, 1, 200);
            }
            else
            {
                sample = sampler.GetSample(m_processId, 1, 200, extraDetails);

            }
            return sample;
        }

        public void StopListening()
        {
            if (m_process != null)
            {
                //remove event registrations
                m_blnIsListening = false;
                m_process.PostDebugEvent -= m_processEventHandler;
                m_processEventHandler = null;

                //detach from the process
                Task t = Task.Factory.StartNew(() =>
                    {
                        if (m_process.IsAlive)
                        {
                            m_process.CorProcess.Stop(0);
                            m_process.Detach();
                        }
                    });
                t.Wait();
            }

        }



        //public bool CollectCallStacksNow { get; set; }

        //public bool CollectPerfSampleNow { get; set; }
    }
}
