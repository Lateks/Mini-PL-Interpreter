using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using AST;
using Errors;

namespace MiniPlInterpreter
{
    public class InterpretingNodeVisitor : NodeVisitor
    {
        private SymbolTable symboltable;
        private Stack operands;
        public Hashtable Valuetable
        {
            get;
            private set;
        }

        public InterpretingNodeVisitor(SymbolTable symboltable)
        {
            this.symboltable = symboltable;
            Valuetable = new Hashtable();
            this.operands = new Stack();
        }

        public void run(Program program)
        {
            program.accept(this);
        }

        public void visit(Program node) { }

        public void visit(IntegerLiteral node)
        {
            int literal = Convert.ToInt32(node.Value);
            operands.Push(literal);
        }

        public void visit(StringLiteral node)
        {
            operands.Push(node.Value);
        }

        public void visit(VariableDeclaration node)
        { // set default values
            if (node.Type == "int")
                Valuetable[symboltable.resolve(node.Name)] = 0;
            else if (node.Type == "bool")
                Valuetable[symboltable.resolve(node.Name)] = false;
            else
                Valuetable[symboltable.resolve(node.Name)] = "";
        }

        public void visit(VariableReference node)
        {
            operands.Push(symboltable.resolve(node.Name));
        }

        public void visit(ArithmeticOp node)
        {
            int secondop = pop<int>();
            int firstop = pop<int>();
            switch (node.OpSymbol)
            {
                case "+":
                    operands.Push(firstop + secondop);
                    break;
                case "-":
                    operands.Push(firstop - secondop);
                    break;
                case "*":
                    operands.Push(firstop * secondop);
                    break;
                case "/":
                    operands.Push(firstop / secondop);
                    break;
            }
        }

        T pop<T>()
        {
            dynamic value = operands.Pop();
            if (value is T)
                return value;
            else
                return (T) Valuetable[value];
        }

        public void visit(LogicalOp node)
        {
            dynamic secondop = operands.Pop();
            dynamic firstop = operands.Pop();
            switch (node.OpSymbol)
            {
                case "=":
                    operands.Push(firstop == secondop);
                    break;
                case "&":
                    operands.Push(firstop && secondop);
                    break;
            }
        }

        public void visit(UnaryNot node)
        {
            bool operand = pop<bool>();
            operands.Push(!operand);
        }

        public void visit(Loop node)
        {
            int end = pop<int>();
            int begin = pop<int>();
            Symbol loopvariable = pop<Symbol>();

            for (int i = begin; i <= end; i++)
            {
                Valuetable[loopvariable] = i;
                foreach (Statement statement in node.LoopBody)
                {
                    statement.accept(this);
                }
            }
        }

        public void visit(Range node) { /*no op, range operands already in stack*/ }

        public void visit(Assignment node)
        {
            dynamic value = operands.Pop();
            Symbol variable;
            if (node.Variable is VariableReference)
                variable = pop<Symbol>();
            else
                variable = symboltable.resolve(node.VarName);
            Valuetable[variable] = value;
        }

        public void visit(ExpressionStatement node)
        {
            dynamic expression = operands.Pop();
            dynamic value;
            if (expression is Symbol)
                value = Valuetable[expression];
            else
                value = expression;

            if (node.Keyword == "assert" && !value)
            {
                if (expression is Symbol)
                    throw new AssertionFailed("Assertion failed: " + expression + " is false.");
                else // TODO: write a better error message (e.g. row/col information?)
                    throw new AssertionFailed("Assertion failed.");
            }
            if (node.Keyword == "print")
                Console.WriteLine(value);
        }

        public void visit(ReadStatement node)
        {
            Symbol variable = pop<Symbol>();
            string input = Console.ReadLine(); // TODO: implement reading single words
            if (variable.Type == "int")
            {
                try
                {
                    int value = Convert.ToInt32(input);
                    Valuetable[variable] = value;
                }
                catch (FormatException)
                {
                    throw new ReadError("Could not convert input \"" + input + "\" to int.");
                }
                catch (OverflowException)
                {
                    throw new ReadError("Integer overflow when converting input \"" + input + "\" to int.");
                }
            }
            else // string variable
            {
                Valuetable[variable] = input;
            }
        }
    }
}
