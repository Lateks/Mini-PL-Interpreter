using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniPLInterpreter
{
    // A simple buffered input reader for use with the Mini-PL read statement.
    // Specification says that read reads one whitespace-delimited word at
    // a time. If the input row for read contains more whitespace-delimited
    // words than one, the rest of the line is buffered and used the next
    // time read is called.
    public class InputReader
    {
        string readbuffer;
        static char[] whitespace = { '\t', '\r', '\n', ' ', '\v', '\f' };

        public InputReader()
        {
            this.readbuffer = "";
        }

        public string ReadWord()
        {
            if (readbuffer == "")
                readbuffer = Console.ReadLine().Trim();

            var split = readbuffer.Split(whitespace, 2, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length < 2)
                readbuffer = "";
            else
                readbuffer = split[1];

            if (split.Length >= 1)
                return split[0];
            else
                return "";
        }
    }
}
