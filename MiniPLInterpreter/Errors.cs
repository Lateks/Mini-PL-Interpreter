using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Errors
{
    public class LexicalError : Exception
    {
        public string Message
        {
            get;
            private set;
        }

        public LexicalError(string message)
        {
            Message = message;
        }
    }

    public class SyntaxError : Exception
    {
        public string Message
        {
            get;
            private set;
        }

        public SyntaxError(string message)
        {
            Message = message;
        }
    }

    public class SemanticError : Exception
    {
        public string Message
        {
            get;
            private set;
        }

        public SemanticError(string message)
        {
            Message = message;
        }
    }
}
