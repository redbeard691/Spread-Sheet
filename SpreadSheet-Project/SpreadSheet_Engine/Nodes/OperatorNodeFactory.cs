// <copyright file="OperatorNodeFactory.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ExpressionTree.Nodes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using SpreadSheet_Engine.Nodes;

    /// <summary>
    /// My Operator Node Factory.
    /// </summary>
    public class OperatorNodeFactory
    {
        /// <summary>
        /// Holds each available operator type.
        /// </summary>
        private Dictionary<char, Type> operators = new ()
        {
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="OperatorNodeFactory"/> class.
        /// </summary>
        public OperatorNodeFactory()
        {
            this.TraverseAvailableOperators((op, type) => this.operators.Add(op, type));
        }

        /// <summary>
        /// Delegator.
        /// </summary>
        /// <param name="op">operator character.</param>
        /// <param name="type">operator type.</param>
        private delegate void OnOperator(char op, Type type);

        /// <summary>
        /// Create operatore node.
        /// </summary>
        /// <param name="op"> operator character.</param>
        /// <returns> returns one of the operations. </returns>
        public OperatorNode? CreateOperatorNode(char op)
        {
            if (operators.ContainsKey(op))
            {
                object operatorNodeObject = Activator.CreateInstance(operators[op]);
                if (operatorNodeObject is OperatorNode)
                {
                    return (OperatorNode)operatorNodeObject;
                }
                else
                {
                    throw new Exception("Unhandled Operator.");
                }
            }

            return null;
        }

        /// <summary>
        /// Checks for associativity between left and right nodes.
        /// </summary>
        /// <param name="ch">character operator.</param>
        /// <returns>associativity.</returns>
        public OperatorNode.Associative Associativity(char ch)
        {
            OperatorNode.Associative associative = OperatorNode.Associative.Left;

            if (this.IsOperator(ch))
            {
                Type type = this.operators[ch];
                PropertyInfo propertyInfo = type.GetProperty("Associativity");
                if (propertyInfo != null)
                {
                    object value = propertyInfo.GetValue(type);
                    if (value.GetType().Name == "Associative")
                    {
                        associative = (OperatorNode.Associative)value;
                    }
                }
            }

            return associative;
        }

        /// <summary>
        /// Returns specific operand precendence
        /// </summary>
        /// <param name="op"> operator read in.</param>
        /// <returns>precedence integer.</returns>
        public int GetPrecedence(char op)
        {
            int outPut = -1;

            if (this.IsOperator(op))
            {
                if (op == '+')
                {
                    AdditionNode addNode= new AdditionNode();
                    outPut = addNode.ReturnPrecedence();
                }
                else if (op == '-')
                {
                    SubtractionNode subtractionNode = new SubtractionNode();
                    outPut = subtractionNode.ReturnPrecedence();
                }
                else if (op == '*')
                {
                  MultiplicationNode multiplicationNode = new MultiplicationNode();
                  outPut = multiplicationNode.ReturnPrecedence();
                }
                else if (op == '/')
                {
                    DivisionNode divisionNode = new DivisionNode();
                    outPut = divisionNode.ReturnPrecedence();
                }
            }

            return outPut;
        }

        /// <summary>
        /// Finds if character is an operator or not.
        /// </summary>
        /// <param name="op">reads in character.</param>
        /// <returns> return true or false.</returns>
        public bool IsOperator (char op)
        {
            if (operators.ContainsKey(op))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Traverses all the available operators.
        /// </summary>
        /// <param name="onOperator">Currently highlighted operator.</param>
        private void TraverseAvailableOperators(OnOperator onOperator)
        {
            Type operatorNodeType = typeof(OperatorNode);

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                IEnumerable<Type> operatorTypes =
                    assembly.GetTypes().Where(type => type.IsSubclassOf(operatorNodeType));

                foreach (var type in operatorTypes)
                {
                    PropertyInfo operatorField = type.GetProperty("Operator");
                    if (operatorField != null)
                    {
                        object value = operatorField.GetValue(type);
                        if (value is char)
                        {
                            char operatorSymbol = (char)value;
                            onOperator(operatorSymbol, type);
                        }
                    }
                }
            }
        }
    }
}
