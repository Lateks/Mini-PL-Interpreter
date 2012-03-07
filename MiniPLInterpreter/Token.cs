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

    public class StringToken : Token
    {
        private string value;
        public string Value
        {
            get { return value; }
        }

        public StringToken(string name, int row, int col)
            : base(row, col)
        {
            this.value = name;
        }
    }

    public class IntegerLiteral : StringToken
    {
        public IntegerLiteral(string value, int row, int col)
            : base(value, row, col) { }
    }

    public class StringLiteral : StringToken
    {
        public StringLiteral(string value, int row, int col)
            : base(value, row, col) { }
    }

    public class Identifier : StringToken
    {
        public Identifier(string name, int row, int col)
            : base(name, row, col) { }
    }

    public class Keyword : StringToken
    {
        public Keyword(string name, int row, int col)
            : base(name, row, col) { }
    }

    public class Type : Keyword
    {
        public Type(string name, int row, int col)
            : base(name, row, col) { }        
    }

    public class Operator : StringToken
    {
        public Operator(string symbol, int row, int col)
            : base(symbol, row, col) { }
    }

    public class RangeOperator : Token
    {
        public RangeOperator(int row, int col)
            : base(row, col) { }
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

    public class EOF : Token
    {
        public EOF(int row, int col) : base(row, col) { }
    }
}