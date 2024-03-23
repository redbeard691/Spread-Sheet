// <copyright file="ExpressionTreeClass.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ExpressionTree
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.Design.Serialization;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Security.AccessControl;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using ExpressionTree.Nodes;

    /// <summary>
    /// Expression tree used for SpreadSheet_Engine.
    /// </summary>
    public class ExpressionTreeClass
    {
        private readonly Node? root;

        private readonly string expression = string.Empty;

        private Dictionary<string, double> variables = new Dictionary<string, double>();

        private OperatorNodeFactory opNodeFactory = new OperatorNodeFactory();

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionTreeClass"/> class.
        /// </summary>
        public ExpressionTreeClass()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionTreeClass"/> class.
        /// </summary>
        /// <param name="expression"> expression. </param>
        public ExpressionTreeClass(string expression)
        {
            expression.Trim();
            this.root = this.BuildExpression(expression);
        }

        /// <summary>
        /// Calls private Evaluate().
        /// </summary>
        /// <returns>Complete constructed method.</returns>
        public double Evaluate()
        {
                return this.root!.Evaluate();
        }

        /// <summary>
        /// Sets variables name and value in dictionary and replaces dublicates.
        /// from the specific expression.
        /// </summary>
        /// <param name="name"> name. </param>
        /// <param name="value"> value. </param>
        public void SetVariable(string name, double value)
        {
            if (this.variables.ContainsKey(name.ToUpper()))
            {
                this.variables.Remove(name.ToUpper());
                this.variables.Add(name.ToUpper(), value);
            }
            else
            {
                this.variables.Add(name.ToUpper(), value);
            }
        }

        /// <summary>
        /// Gets the variables(s) within the cell in question.
        /// </summary>
        /// <returns>Returns a list of variables..</returns>
        public List<string> GetVariables()
        {
            List<string> temp = new List<string>();

            foreach (var pair in this.variables)
            {
                temp.Add(pair.Key);
            }

            return temp;
        }

        /// <summary>
        /// Does complicated math cause.
        /// </summary>
        /// <param name="expression">User input.</param>
        /// <returns>return postfix expression.</returns>
        public Queue<string> ShuntingYardAlgorithm(string expression)
        {
            Queue<string> postFixExpression = new Queue<string>();
            Stack<char> operatorStack = new Stack<char>();
            int startOperand = -1;

            for (int index = 0; index < expression.Length; index++)
            {
                char c = expression[index];

                // Check if char is Operator or Parenthese.
                if (this.OperatorOrParentheses(c))
                {
                    if (startOperand != -1)
                    {
                        string operand = expression.Substring(startOperand, index - startOperand);
                        postFixExpression.Enqueue(operand);
                        startOperand = -1;
                    }

                    // if left parenthese.
                    if (c == '(')
                    {
                        operatorStack.Push(c);
                    }

                    // if right parenthese
                    else if (c == ')')
                    {
                        char op = operatorStack.Pop();

                        // Poping operators off stack into queu till righ parenthese found.
                        while (op != '(')
                        {
                            postFixExpression.Enqueue(op.ToString());
                            op = operatorStack.Pop();
                        }
                    }
                    else if (this.opNodeFactory.IsOperator(c))
                    {
                        if (operatorStack.Count == 0 || c == '(')
                        {
                            operatorStack.Push(c);
                        }
                        else if (this.HigherPrecedence(c, operatorStack.Peek()) ||
                                 (this.SamePrecedence(c, operatorStack.Peek()) && this.RightAssociative(c)))
                        {
                            operatorStack.Push(c);
                        }
                        else if (this.LowerPrecedence(c, operatorStack.Peek()) ||
                                 (this.SamePrecedence(c, operatorStack.Peek()) && this.LeftAssociative(c)))
                        {
                            do
                            {
                                char stackOp = operatorStack.Pop();
                                postFixExpression.Enqueue(stackOp.ToString());
                            }
                            while (operatorStack.Count > 0 && (this.LowerPrecedence(c, operatorStack.Peek())
                                || (this.SamePrecedence(c, operatorStack.Peek()) && this.LeftAssociative(c))));
                            operatorStack.Push(c);
                        }
                        else
                        {
                            postFixExpression.Enqueue(c.ToString());
                        }
                    }
                }
                else if (startOperand == -1)
                {
                    startOperand = index;
                }
            }

            if (startOperand != -1)
            {
                postFixExpression.Enqueue(expression.Substring(startOperand, expression.Length - startOperand));
                startOperand = -1;
            }

            while (operatorStack.Count > 0)
            {
                postFixExpression.Enqueue(operatorStack.Pop().ToString());
            }

            return postFixExpression;
        }

        private bool LeftAssociative(char expChar)
        {
            if (this.opNodeFactory.Associativity(expChar) == OperatorNode.Associative.Left)
            {
                return true;
            }

            return false;
        }

        private bool RightAssociative(char expChar)
        {
            if (this.opNodeFactory.Associativity(expChar) == OperatorNode.Associative.Right)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// builds arthmic expression using shunting yard algorithm.
        /// </summary>
        /// <param name="expression">user input.</param>
        /// <returns>return built expression.</returns>
        private Node BuildExpression(string expression)
        {
            Stack<Node> stackNodes = new Stack<Node>();
            var postFixExpression = this.ShuntingYardAlgorithm(expression);

            foreach (var index in postFixExpression)
            {
                if (index.Length == 1 && this.OperatorOrParentheses(index[0]))
                {
                    OperatorNode node = this.opNodeFactory.CreateOperatorNode(index[0]);
                    node.Right = stackNodes.Pop();
                    node.Left = stackNodes.Pop();
                    stackNodes.Push(node);
                }
                else
                {
                    if (double.TryParse(index, out double num))
                    {
                        stackNodes.Push(new ConstantNode(num));
                    }
                    else
                    {
                        this.variables.Add(index.ToUpper(), 0);
                        stackNodes.Push(new VariableNode(index, ref this.variables));
                    }
                }
            }

            return stackNodes.Pop();
        }

        /// <summary>
        /// Evaluates if expression operator has same precedence
        /// then stack operator.
        /// </summary>
        /// <param name="expchar">expression operator.</param>
        /// <param name="stackchar">stack operator.</param>
        /// <returns>true or false.</returns>
        private bool SamePrecedence(char expchar, char stackchar)
        {
            if (this.opNodeFactory.GetPrecedence(expchar) == this.opNodeFactory.GetPrecedence(stackchar))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Evaluates if expression operator has higher precedence
        /// then stack operator.
        /// </summary>
        /// <param name="expchar">Operator from expression.</param>
        /// <param name="stackchar">Operator from stack.</param>
        /// <returns>True or False.</returns>
        private bool HigherPrecedence(char expchar, char stackchar)
        {
            if (this.opNodeFactory.GetPrecedence(expchar) > this.opNodeFactory.GetPrecedence(stackchar))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Evaluate if expression operator has lower precedence
        /// then stack operator.
        /// </summary>
        /// <param name="expchar"> expression operator.</param>
        /// <param name="stackchar">stack operator.</param>
        /// <returns>true or false.</returns>
        private bool LowerPrecedence(char expchar, char stackchar)
        {
            if (this.opNodeFactory.GetPrecedence(expchar) < this.opNodeFactory.GetPrecedence(stackchar))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks to see if the character is an operator or
        /// a Parentheses.
        /// </summary>
        /// <param name="c">Reads in expression character.</param>
        /// <returns>true or false.</returns>
        private bool OperatorOrParentheses(char c)
        {
            char[] parantheses = { '(', ')' };
            if (parantheses.Contains(c) || this.opNodeFactory.IsOperator(c))
            {
                return true;
            }

            return false;
        }
    }
}