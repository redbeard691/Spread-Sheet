// <copyright file="AdditionNode.cs" company="PlaceholderCompany">
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
    /// Addition Node class for operator.
    /// </summary>
    internal class AdditionNode : OperatorNode
    {
        private readonly int precedence = 3;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdditionNode"/> class.
        /// </summary>
        public AdditionNode()
        {
        }

        /// <summary>
        /// Gets operator Property.
        /// </summary>
        public static char Operator => '+';

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
        /// Addition of nodes gets returned.
        /// </summary>
        /// <returns> leftnode+rightnode.</returns>
        public override double Evaluate()
        {
            try
            {
                return this.Left.Evaluate() + this.Right.Evaluate();
            }
            catch (Exception)
            {
                Console.WriteLine("Error with addition of left or right nodes.");
                throw new Exception("Child nodes are not constant or value was not set.");
            }
        }
    }
}
