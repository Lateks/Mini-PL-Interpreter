using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniPLInterpreter
{
    public class SymbolTable
    {
        private Dictionary<string, Symbol> symboltable;

        public SymbolTable()
        {
            symboltable = new Dictionary<string, Symbol>();
        }

        public void define(Symbol sym)
        {
            symboltable.Add(sym.Name, sym);
        }

        public Symbol resolve(string name)
        {
            try
            {
                return symboltable[name];
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }
    }

    public class Symbol
    {
        public string Name
        {
            get;
            private set;
        }
        public string Type
        {
            get;
            private set;
        }

        public Symbol(string name, string type)
        {
            Name = name;
            Type = type;
        }
    }
}
