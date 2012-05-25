using System;
using System.Runtime.CompilerServices;

namespace CallerInfo
{
    public class Logger
    {
        private readonly string _className;

        private Logger(string mayBeClassName)
        {
            _className = mayBeClassName;
        }

        public static Logger Get([CallerMemberName]string memberName = "")
        {
            return new Logger(memberName);
        }

        public void Log(string message, [CallerMemberName]string memberName = "", [CallerLineNumber]int lineNumber = 0)
        {
            Console.WriteLine("[{0}.{1} - {2}] - {3}", _className, memberName, lineNumber, message);            
        }
    }
}
