using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProcessExaminator
{
    public class StackSample
    {
        public DateTime Time { get; set; }
        public int ThreadId { get; set; }
        private List<string> m_callstack = new List<string>();
        public List<string> CallStack { get { return m_callstack; } set { m_callstack = value; } }
        public override string ToString()
        {
            StringBuilder callstackSb =  new StringBuilder();
            CallStack.ForEach(line => callstackSb.AppendLine(line));
            return callstackSb.ToString();
        }
    }
}
