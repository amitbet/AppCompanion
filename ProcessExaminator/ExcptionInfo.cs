using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProcessExaminator
{
    public class ExceptionInfo
    {
        string m_message;
        string m_source;
        string m_callstack;
        string m_function;
        string m_exType;

        public string Message
        {
            get { return m_message; }
            set { m_message = value; }
        }

        public string Source
        {
            get { return m_source; }
            set { m_source = value; }
        }

        public string Callstack
        {
            get { return m_callstack; }
            set { m_callstack = value; }
        }

        public string Function
        {
            get { return m_function; }
            set { m_function = value; }
        }

        public string ExType
        {
            get { return m_exType; }
            set { m_exType = value; }
        }
    }
}
