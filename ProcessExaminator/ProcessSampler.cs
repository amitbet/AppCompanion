using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Samples.Debugging.MdbgEngine;
using System.Diagnostics;
using Microsoft.Samples.Debugging.CorDebug;
using System.Threading;
using System.IO;
using System.Reflection;

namespace ProcessExaminator
{
    public class ProcessSampler
    {


        int m_intMainThreadId = -1;
        bool m_onlyMainThread = false;


        public ProcessSampler(bool onlyMainThread)
        {
            m_onlyMainThread = onlyMainThread;
        }


        public List<ProcessSample> GetSample(int pid, int numOfSamples, int timeBetweenSamplesInMillis, bool extraDetails = false, bool originFirst = false)
        {
            List<ProcessSample> sample = null;
            MDbgProcess proc = null;
            try
            {
                MDbgEngine debugger = new MDbgEngine();

                proc = DebuggerUtils.AttachToProcess(pid, debugger);

                sample = GetSample(proc, numOfSamples, timeBetweenSamplesInMillis, extraDetails, originFirst);
            }
            finally
            {
                if (proc != null)
                    proc.Detach().WaitOne();
            }
            return sample;
        }

        /// <summary>
        /// returns a list of samples (in time) of list (per thread) of CallstackSamples
        /// </summary>
        /// <param name="pid">the process id to attach to</param>
        /// <param name="numOfSamples"></param>
        /// <param name="timeBetweenSamplesInMillis"></param>
        /// <returns></returns>
        public List<ProcessSample> GetSample(MDbgProcess proc, int numOfSamples, int timeBetweenSamplesInMillis, bool extraDetails = false, bool originFirst = false)
        {
            List<ProcessSample> samples = new List<ProcessSample>();

            try
            {
                m_intMainThreadId = -1;

                if (m_onlyMainThread)
                {
                    MDbgThread mainthread = GetMainThread(proc);

                    m_intMainThreadId = mainthread.Id;
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
                }

                if (m_intMainThreadId == -1)
                {
                    m_onlyMainThread = false;
                }

                Console.WriteLine("startTime: " + DateTime.Now.ToString("hh:mm:ss.fff"));
                int sleepTime = timeBetweenSamplesInMillis;
                Stopwatch s = Stopwatch.StartNew();
                for (int i = 0; i < numOfSamples; i++)
                {

                    ProcessSample frames = GetCallstacks(proc, m_onlyMainThread, extraDetails, originFirst);
                    samples.Add(frames);
                    //samples.Insert(0,frames);
                    s.Reset();
                    s.Start();
                    proc.Go();
                    //sleepTime -= (int)s.ElapsedMilliseconds;


                    //sleep
                    if (sleepTime > 0)
                        Thread.Sleep(sleepTime);

                    //reset sleep time
                    sleepTime = timeBetweenSamplesInMillis;

                    s.Reset();
                    s.Start();
                    proc.AsyncStop().WaitOne();
                    //sleepTime -= (int)s.ElapsedMilliseconds;
                }
                Console.WriteLine("EndTime: " + DateTime.Now.ToString("hh:mm:ss.fff"));
            }
            finally
            {
                //if (proc != null) { proc.Detach().WaitOne(); }
            }

            return samples;
        }

        private MDbgThread GetMainThread(MDbgProcess proc)
        {
            MDbgThread mainthread = null;
            foreach (MDbgThread thread in proc.Threads)
            {
                if (DebuggerUtils.CheckValidState(thread))
                {
                    MDbgFrame[] frames = new MDbgFrame[0];

                    try
                    {
                        frames = thread.Frames.ToArray();
                    }
                    catch { }

                    foreach (MDbgFrame f in frames)
                    {
                        string line = DebuggerUtils.GetFrameString(f);

                        if (line.Contains("System.Windows.Application.Run") ||
                            line.Contains("Program.Main") ||
                            line.Contains("System.Windows.Forms.Application.Run"))
                        {
                            mainthread = thread;
                            //m_intMainThreadId = thread.Id;
                            break;
                        }
                    }
                }
            }
            return mainthread;
        }

        public void WriteStacksToFile(List<ProcessSample> samples)
        {
            string strRunDir = DebuggerUtils.GetOutputDirectory();
            string filename = DebuggerUtils.GetTimeString(true) + ".stacks.txt";
            string strLogPath = Path.Combine(strRunDir, filename);
            int idx = 0;
            using (StreamWriter wr = new StreamWriter(strLogPath))
            {
                foreach (ProcessSample prSample in samples)
                {
                    wr.WriteLine("=========== sampleId:" + idx++ + "===========");
                    foreach (StackSample thSample in prSample.Samples)
                    {
                        wr.WriteLine("----------- thread:" + thSample.ThreadId + " time:" + thSample.Time.ToString("dd.MM.yyyy hh:mm:ss.fff") + "-----------");
                        foreach (string line in thSample.CallStack)
                        {
                            wr.WriteLine(line);
                        }
                    }
                }
            }
        }

        public List<StackTreeNode> GetMostTravelledPathInTree(StackTreeNode treeRoot)
        {
            List<StackTreeNode> retVal = new List<StackTreeNode>();
            StackTreeNode currentNode = treeRoot;
            retVal.Add(currentNode);

            while (currentNode.Children.Count > 0)
            {
                int maxAppearences = currentNode.Children.Max(c => c.StackAppearancesCounter);
                //choose the most travelled node
                currentNode = currentNode.Children.Where(c => c.StackAppearancesCounter == maxAppearences).FirstOrDefault();
                retVal.Add(currentNode);
            }
            return retVal;
        }
        /// <summary>
        /// creates a stack tree for each thread in samples
        /// </summary>
        /// <param name="samples"></param>
        /// <returns>a dictionary of [threadId,StackTreeRoot]</returns>
        public Dictionary<int, StackTreeNode> CreateStackTrees(List<ProcessSample> samples)
        {
            Dictionary<int, StackTreeNode> retval = new Dictionary<int, StackTreeNode>();
            //go over the process samples over time
            foreach (ProcessSample prSample in samples)
            {
                //go over threads in each sample
                foreach (StackSample thSample in prSample.Samples)
                {

                    StackTreeNode rootNode = null;
                    if (!retval.ContainsKey(thSample.ThreadId))
                    {
                        rootNode = new StackTreeNode();
                        retval.Add(thSample.ThreadId, rootNode);
                    }
                    else
                    {
                        rootNode = retval[thSample.ThreadId];
                    }

                    //go over lines in each thread sample
                    StackTreeNode prevNode = rootNode;
                    string prevLine = null;
                    foreach (string line in thSample.CallStack)
                    {
                        prevNode = prevNode.AddToSubTree(prevLine, line);
                        prevLine = line;
                        ++prevNode.StackAppearancesCounter;
                    }
                }
            }
            return retval;
        }



        private ProcessSample GetCallstacks(MDbgProcess proc, bool onlyMainThread , bool withArgs = false, bool originFirst = false)
        {
            if (onlyMainThread)
                return DebuggerUtils.GetCallstacks(proc, m_intMainThreadId, withArgs, originFirst);
            else
                return DebuggerUtils.GetCallstacks(proc, -1, withArgs, originFirst);
        }

  

    }
}
