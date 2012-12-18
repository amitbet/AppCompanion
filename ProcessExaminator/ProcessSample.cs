using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProcessExaminator
{
    public class ProcessSample
    {
        private List<StackSample> m_samples = new List<StackSample>();

        public int ProcessId { get; set; }
        public DateTime Time { get; set; }
        public List<StackSample> Samples
        {
            get { return m_samples; }
            set { m_samples = value; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (StackSample stack in Samples)
            {
                sb.AppendLine(stack.Time.ToString("dd/MM/yyyy HH:mm:ss.fff") + " ThreadId: " + stack.ThreadId);
                stack.CallStack.ForEach(line=>sb.AppendLine(line));
                sb.AppendLine();
            }
            return sb.ToString();
        }

    }
}
