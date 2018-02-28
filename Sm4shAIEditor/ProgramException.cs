using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sm4shAIEditor
{
    class ProgramException : Exception
    {
        public ProgramException()
            : base() { }
        public ProgramException(string message)
            : base(message) { }
        public ProgramException(string message, string arg)
            : base(string.Format(message, arg)) { }
    }
}
