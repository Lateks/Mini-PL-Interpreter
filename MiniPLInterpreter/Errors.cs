using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Errors
{
    public class LexicalError : System.Exception
    {
        public LexicalError(string message) { }
    }

    public class SyntaxError : Exception
    {
        public SyntaxError(string message) { }
    }
}
