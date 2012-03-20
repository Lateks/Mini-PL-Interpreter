using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using MiniPLInterpreter.Support.Symbols;
using MiniPLInterpreter.Support.AbstractSyntaxTree;
using MiniPLInterpreter.Errors.MiniPL;
using MiniPLInterpreter.Support;

namespace MiniPLInterpreter
{
    namespace Interpreter
    {
        // This visitor traverses and executes the program AST.
        // Note that the typechecker needs to have been run before
        // interpreting the AST (to check types as well as to
        // build the symbol table).
        //
        // The visitor keeps track of values in two Hashtables.
        // One of them (nodevalues) contains the evaluated values
        // of AST nodes and is keyed by Node. Unlike in the type
        // checker, a stack would have been more difficult to
        // use for storing these values because their ordering
        // is often more important (e.g. left hand and right hand
        // sides of certain arithmetic operations). In practice
        // this contains values that are not (or at least not yet)
        // assigned to a variable.
        //
        // The other one (Valuetable) contains the values of symbols
        // defined in the symbol table after they are assigned and
        // is keyed by Symbol objects fetched from the symbol table.
        public class InterpretingNodeVisitor : NodeVisitor
        {
            private SymbolTable symboltable;
            private InputReader inputreader;
            private Hashtable nodevalues; // For storing evaluated values of nodes
            // (using Node objects as keys).
            public Hashtable Valuetable
            {
                get;
                private set;
            }

            public InterpretingNodeVisitor(SymbolTable symboltable)
            {
                this.symboltable = symboltable;
                inputreader = new InputReader();
                Valuetable = new Hashtable();
                nodevalues = new Hashtable();
            }

            public void Run(Program program)
            {
                program.accept(this);
            }

            public void visit(Program node) { }

            public void visit(IntegerLiteral node)
            {
                int literal = Convert.ToInt32(node.Value);
                nodevalues[node] = literal;
            }

            public void visit(StringLiteral node)
            {
                nodevalues[node] = node.Value;
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
                nodevalues[node] = symboltable.resolve(node.Name);
            }

            public void visit(ArithmeticOp node)
            {
                int firstop = fetch<int>(node.LeftOp);
                int secondop = fetch<int>(node.RightOp);

                switch (node.OpSymbol)
                {
                    case "+":
                        nodevalues[node] = firstop + secondop;
                        break;
                    case "-":
                        nodevalues[node] = firstop - secondop;
                        break;
                    case "*":
                        nodevalues[node] = firstop * secondop;
                        break;
                    case "/":
                        if (secondop == 0)
                            throw new MiniPLDivisionByZero("Division by zero on row " + node.Row + ".");
                        nodevalues[node] = firstop / secondop;
                        break;
                }
            }

            // Fetches the current evaluated value of the given node.
            // The type parameter specifies the expected type of the
            // node (for casting possible variable values to the correct
            // type). Note that typechecking has already been performed
            // prior to running this visitor, so type errors at this
            // stage do not happen.
            T fetch<T>(Node key)
            {
                dynamic value = nodevalues[key];
                if (value is T)
                    return value;
                else // value is a Symbol but expected type is something else
                    return (T)Valuetable[value];
            }

            public void visit(LogicalOp node)
            {
                dynamic firstop = nodevalues[node.LeftOp];
                dynamic secondop = nodevalues[node.RightOp];

                switch (node.OpSymbol)
                {
                    case "=":
                        nodevalues[node] = firstop == secondop;
                        break;
                    case "&":
                        nodevalues[node] = firstop && secondop;
                        break;
                }
            }

            public void visit(UnaryNot node)
            {
                bool operand = fetch<bool>(node.Operand);
                nodevalues[node] = !operand;
            }

            public void visit(Loop node)
            {
                int begin = fetch<int>(node.Range.Begin);
                int end = fetch<int>(node.Range.End);
                Symbol loopvariable = fetch<Symbol>(node.Variable);

                for (int i = begin; i <= end; i++)
                {
                    Valuetable[loopvariable] = i;
                    foreach (Statement statement in node.LoopBody)
                    {
                        statement.accept(this);
                    }
                }
            }

            public void visit(Range node) { }

            public void visit(Assignment node)
            {
                Symbol variable;
                if (node.Variable is VariableReference)
                    variable = fetch<Symbol>(node.Variable);
                else
                    variable = symboltable.resolve(node.VarName);

                dynamic value = nodevalues[node.Expression];
                if (value is Symbol)
                    Valuetable[variable] = Valuetable[value];
                else
                    Valuetable[variable] = value;
            }

            public void visit(ExpressionStatement node)
            {
                dynamic expression = nodevalues[node.Expression];
                dynamic value;
                if (expression is Symbol)
                    value = Valuetable[expression];
                else
                    value = expression;

                if (node.Keyword == "assert" && !value)
                {
                    if (expression is Symbol)
                        throw new MiniPLAssertionFailed("Assertion failed: variable \"" +
                            ((Symbol)expression).Name + "\" on row " + node.Row + "is false.");
                    else
                        throw new MiniPLAssertionFailed("Assertion failed on row " + node.Row + ".");
                }
                if (node.Keyword == "print")
                    Console.Write(value);
            }

            public void visit(ReadStatement node)
            {
                Symbol variable = fetch<Symbol>(node.Variable);
                string input = inputreader.ReadWord();
                if (variable.Type == "int")
                {
                    try
                    {
                        int value = Convert.ToInt32(input);
                        Valuetable[variable] = value;
                    }
                    catch (FormatException)
                    {
                        throw new MiniPLReadError("Could not convert input \"" + input + "\" to int.");
                    }
                    catch (OverflowException)
                    {
                        throw new MiniPLReadError("Integer overflow when converting input \"" + input + "\" to int.");
                    }
                }
                else // string variable
                {
                    Valuetable[variable] = input;
                }
            }
        }
    }
}