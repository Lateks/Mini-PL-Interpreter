using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AST;
using Errors;

namespace MiniPlInterpreter
{
    public class SemanticAnalyser
    {
    }

    public class SymbolTable
    {
        Dictionary<string, Symbol> symboltable;

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
            return symboltable[name];
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

    public class TypeCheckingVisitor : NodeVisitor
    {
        SymbolTable symboltable;

        public TypeCheckingVisitor()
        {
            symboltable = new SymbolTable();
        }

        public SymbolTable BuildSymbolTableAndTypeCheck(Program node)
        {
            node.accept(this);
            return symboltable;
        }

        public void visit(VariableDeclaration node)
        {
            if (symboltable.resolve(node.Name) != null)
                throw new SemanticError("Variable " + node.Name + " is already defined.");
            Symbol symbol = new Symbol(node.Name, node.Type);
            symboltable.define(symbol);
        }

        public void visit(Variable node)
        {
            if (symboltable.resolve(node.Name) == null)
                throw new SemanticError("Reference to undefined identifier " + node.Name + ".");
        }

        public void visit(ArithmeticOp node)
        {
        }

        public void visit(LogicalOp node)
        {
        }

        public void visit(Program node) { }
        public void visit(IntegerLiteral node) { }
        public void visit(StringLiteral node) { }
        public void visit(Keyword node) { }
        public void visit(UnaryNot node) { }
        public void visit(Loop node) { }
        public void visit(Range node) { }
        public void visit(Assignment node) { }
        public void visit(ExpressionStatement node) { }
        public void visit(ReadStatement node) { }
    }
}
