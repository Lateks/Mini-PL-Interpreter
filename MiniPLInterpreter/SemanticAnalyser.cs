﻿using System;
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
            try
            {
                return symboltable[name];
            }
            catch (KeyNotFoundException e)
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

    public class TypeCheckingVisitor : NodeVisitor
    {
        SymbolTable symboltable;
        Stack<string> operandtypes;

        public TypeCheckingVisitor()
        {
            symboltable = new SymbolTable();
            operandtypes = new Stack<string>();
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

        public void visit(VariableReference node)
        {
            Symbol var = symboltable.resolve(node.Name);
            if (var == null)
                throw new SemanticError("Reference to undefined identifier " + node.Name + ".");
            else
                operandtypes.Push(var.Type);
        }

        public void visit(Loop node)
        {
            if (symboltable.resolve(node.Variable.Name).Type != "int")
                throw new SemanticError("Loop variable " + node.Variable.Name + " is not an int.");
        }

        public void visit(ArithmeticOp node)
        {
            string rightoptype = operandtypes.Pop();
            string leftoptype = operandtypes.Pop();
            if (rightoptype == "int" && leftoptype == "int")
                operandtypes.Push("int");
            else
                throw new SemanticError("Non-integer arguments to arithmetic operator.");
        }

        public void visit(LogicalOp node)
        {
            // operands may be of any valid type
            operandtypes.Pop();
            operandtypes.Pop();
            operandtypes.Push("bool");
        }

        public void visit(Range node)
        {
            string rightoptype = operandtypes.Pop();
            string leftoptype = operandtypes.Pop();
            if (rightoptype != "int" || leftoptype != "int")
                throw new SemanticError("Invalid argument types for range operator (..).");
        }

        public void visit(Assignment node)
        {
            if (node.Variable is VariableReference)
                operandtypes.Pop();
            string variableType = symboltable.resolve(node.Variable.Name).Type;
            string expressionType = operandtypes.Pop();
            if (variableType != expressionType)
                throw new SemanticError("Incompatible types in assignment.");
        }

        public void visit(UnaryNot node)
        {
            if (operandtypes.Pop() == "bool")
                operandtypes.Push("bool");
            else
                throw new SemanticError("Invalid argument type for unary not operator (!).");
        }

        public void visit(ExpressionStatement node)
        {
            string exprType;
            //if (node.Expression is Variable)
            //    exprType = symboltable.resolve(((Variable)node.Expression).Name).Type;
            //else
            exprType = operandtypes.Pop();

            if (node.Keyword.Name == "assert" && exprType != "bool")
                throw new SemanticError("Invalid argument type for assert statement.");
            else if (node.Keyword.Name == "print" && exprType == "bool")
                throw new SemanticError("Invalid argument type for print statement.");
        }

        public void visit(IntegerLiteral node)
        {
            try
            {
                Convert.ToInt32(node.Value);
                operandtypes.Push("int");
            }
            catch (OverflowException e)
            {
                throw new SemanticError("Integer overflow: " + node.Value);
            }
        }

        public void visit(StringLiteral node)
        {
            operandtypes.Push("string");
        }

        public void visit(Keyword node) { }
        public void visit(ReadStatement node) { }
        public void visit(Program node) { }
    }
}
