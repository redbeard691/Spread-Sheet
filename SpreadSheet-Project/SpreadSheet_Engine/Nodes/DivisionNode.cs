// <copyright file="DivisionNode.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SpreadSheet_Engine.Nodes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using ExpressionTree.Nodes;

    /// <summary>
    /// Division Node class for operator.
    /// </summary>
    internal class DivisionNode : OperatorNode
    {
        private readonly int precedence = 7;

        /// <summary>
        /// Initializes a new instance of the <see cref="DivisionNode"/> class.
        /// </summary>
        public DivisionNode()
        {
        }

        /// <summary>
        /// Gets operator Property.
        /// </summary>
        public static char Operator => '/';

        /// <summary>
        /// Gets associativity Lambda.
        /// </summary>
        public static Associative Associativity => Associative.Left;

        /// <summary>
        /// Return Precedence Value.
        /// </summary>
        /// <returns>precedence value.</returns>
        public int ReturnPrecedence()
        {
            return this.precedence;
        }

        /// <summary>
        /// Division of nodes gets returned.
        /// </summary>
        /// <returns> leftnode/rightnode.</returns>
        public override double Evaluate()
        {
            try
            {
                return this.Left.Evaluate() / this.Right.Evaluate();
            }
            catch (Exception)
            {
                Console.WriteLine("Error with division of left or right nodes.");
                throw new Exception("Child nodes are not constant or value was not set.");
            }
        }
    }
}
