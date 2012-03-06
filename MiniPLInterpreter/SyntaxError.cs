using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Errors
{
    class SyntaxError : Exception
    {
        public SyntaxError(string message) { }
    }
}
