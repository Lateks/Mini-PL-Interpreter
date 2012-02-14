using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TokenTypes
{
    public interface Token
    {
    }

    public class IntegerLiteral : Token
    {
        private int value;
        public int Value
        {
            get { return value; }
        }

        public IntegerLiteral(int value)
        {
            this.value = value;
        }
    }

    public class StringLiteral : Token
    {
        private string value;
        public string Value
        {
            get { return value; }
        }

        public StringLiteral(string value)
        {
            this.value = value;
        }
    }

    public class Keyword : Token
    {
        private string name;
        public string Name
        {
            get { return name; }
        }

        public Keyword(string name)
        {
            this.name = name;
        }
    }

    public class Identifier : Token
    {
        private string name;
        public string Name
        {
            get { return name; }
        }

        public Identifier(string name)
        {
            this.name = name;
        }
    }

    public class LeftParenthesis : Token
    {
        public LeftParenthesis() { }
    }

    public class RightParenthesis : Token
    {
        public RightParenthesis() { }
    }

    public class EndLine : Token
    {
        public EndLine() { }
    }

    public class Operator : Token
    {
        private string symbol;
        public string Symbol
        {
            get { return symbol; }
        }

        public Operator(string symbol)
        {
            this.symbol = symbol;
        }
    }
}