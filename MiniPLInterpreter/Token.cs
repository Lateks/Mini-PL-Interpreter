using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TokenTypes
{
    public class Token
    {
        private int row;
        private int col;

        public int Row
        {
            get { return row; }
        }
        public int Col
        {
            get { return col; }
        }

        public Token(int row, int col)
        {
            this.row = row;
            this.col = col;
        }
    }

    public class IntegerLiteral : Token
    {
        private int value;
        public int Value
        {
            get { return value; }
        }

        public IntegerLiteral(int value, int row, int col)
            : base(row, col)
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

        public StringLiteral(string value, int row, int col)
            : base(row, col)
        {
            this.value = value;
        }
    }

    public class Identifier : Token
    {
        private string name;
        public string Name
        {
            get { return name; }
        }

        public Identifier(string name, int row, int col)
            : base(row, col)
        {
            this.name = name;
        }
    }

    public class Keyword : Identifier
    {
        public Keyword(string name, int row, int col)
            : base(name, row, col) { }
    }

    public class Type : Keyword
    {
        public Type(string name, int row, int col)
            : base(name, row, col) { }        
    }

    public class LeftParenthesis : Token
    {
        public LeftParenthesis(int row, int col)
            : base(row, col) { }
    }

    public class RightParenthesis : Token
    {
        public RightParenthesis(int row, int col)
            : base(row, col) { }
    }

    public class EndLine : Token
    {
        public EndLine(int row, int col)
            : base(row, col) { }
    }

    public class Operator : Token
    {
        private string symbol;
        public string Symbol
        {
            get { return symbol; }
        }

        public Operator(string symbol, int row, int col)
            : base(row, col)
        {
            this.symbol = symbol;
        }
    }
}