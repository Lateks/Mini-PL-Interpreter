using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using AST;

namespace MiniPlInterpreter
{
    public class InterpretingNodeVisitor : NodeVisitor
    {
        private SymbolTable symboltable;
        private Hashtable valuetable;
        private Stack operands;

        public InterpretingNodeVisitor(SymbolTable symboltable)
        {
            this.symboltable = symboltable;
            this.valuetable = new Hashtable();
            this.operands = new Stack();
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

        public void visit(VariableDeclaration node) { }

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
                return (T) valuetable[value];
        }

        public void visit(LogicalOp node)
        {
            bool secondop = pop<bool>();
            bool firstop = pop<bool>();
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
            // Note: In theory variable declarations are never allowed inside loops
            // because there is only one scope (the same variable cannot be declared
            // several times -- which it would be if the loop was iterated over several times).
            // In practice you can write a for loop that only goes through one iteration
            // in which case a variable declaration inside the loop body does not cause
            // problems. Unfortunately at the static type checking phase we have no
            // idea how many times the loop body will be iterated over, so this check must
            // be done at runtime.
            throw new NotImplementedException();
        }

        public void visit(Range node)
        {
            int begin = pop<int>();
            int end = pop<int>();
            operands.Push(Enumerable.Range(begin, end - begin + 1));
        }

        public void visit(Assignment node)
        {
            dynamic value = operands.Pop();
            Symbol variable;
            if (node.Variable is VariableReference)
                variable = pop<Symbol>();
            else
                variable = symboltable.resolve(node.VarName);
            valuetable[variable] = value;
        }

        public void visit(ExpressionStatement node)
        {
            dynamic expression = operands.Pop();
            dynamic value;
            if (expression is Symbol)
                value = valuetable[expression];
            else
                value = expression;

            if (node.Keyword == "assert" && !value)
            {
                throw new NotImplementedException();
                // TODO: write a better message
                // throw an exception to be caught by main
            }
            else // keyword is "print"
                Console.WriteLine(value);
        }

        public void visit(ReadStatement node)
        {
            throw new NotImplementedException();
            Symbol variable = pop<Symbol>();
            string input = Console.ReadLine(); // need to implement reading single words
            if (variable.Type == "int")
            {
                try
                {
                    int value = Convert.ToInt32(input);
                    valuetable[variable] = value;
                }
                catch (FormatException)
                {
                    // throw an exception to be caught by Main
                }
                catch (OverflowException)
                {
                    // throw an exception to be caught by Main
                }
            }
            else // string variable
            {
                // ...
            }
        }
    }
}
