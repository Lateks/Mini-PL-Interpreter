using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Errors
{
    public class LexicalError : Exception
    {
        public LexicalError(string message) { }
    }

    public class SyntaxError : Exception
    {
        public SyntaxError(string message) { }
    }

    public class SemanticError : Exception
    {
        public SemanticError(string message) { }
    }
}
