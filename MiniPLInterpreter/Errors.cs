using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Errors
{
    // Lexical errors are thrown by the scanner.
    public class LexicalError : Exception
    {
        public LexicalError(string message)
            : base(message) { }
    }

    // Syntax errors are thrown by the parser.
    public class SyntaxError : Exception
    {
        public SyntaxError(string message)
            : base(message) { }
    }

    // Semantic errors are thrown by the typechecker.
    public class SemanticError : Exception
    {
        public SemanticError(string message)
            : base(message) { }
    }

    // Mini-PL internal error (thrown by interpreter
    // visitor).
    public class MiniPLAssertionFailed : Exception
    {
        public MiniPLAssertionFailed(string message)
            : base(message) { }
    }

    // Mini-PL internal error (thrown by interpreter
    // visitor).
    public class MiniPLReadError : Exception
    {
        public MiniPLReadError(string message)
            : base(message) { }
    }
}
