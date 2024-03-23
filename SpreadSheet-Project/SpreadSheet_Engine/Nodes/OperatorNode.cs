// <copyright file="OperatorNode.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ExpressionTree.Nodes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Class for Nodes that contain operators.
    /// </summary>
    public abstract class OperatorNode : Node
    {
        // Left CHild
        private Node? left;

        // Right Child
        private Node? right;

        /// <summary>
        /// Initializes a new instance of the <see cref="OperatorNode"/> class.
        /// </summary>
        /// <param name="c"> . </param>
        public OperatorNode()
        {
            // this.Operator = c;
            this.left = null;
            this.right = null;
        }

        /// <summary>
        /// Associative for Left and Right node.
        /// </summary>
        public enum Associative
        {
            /// <summary>
            /// Right Node
            /// </summary>
            Right,

            /// <summary>
            /// Left Node
            /// </summary>
            Left,
        }

        /// <summary>
        /// Gets or sets operator node.
        /// </summary>
        // public char Operator { get; }
        public Node? Left
        {
            get { return this.left; }
            set { this.left = value; }
        }

        /// <summary>
        /// Gets or sets right child of operator.
        /// </summary>
        public Node? Right
        {
            get { return this.right; }
            set { this.right = value; }
        }

        /// <summary>
        /// Throws an excpetion if there is a problem with operator node during evaluation.
        /// </summary>
        /// <returns>nothing.</returns>
        /// <exception cref="NotImplementedException">nothin.</exception>
        public abstract override double Evaluate();
    }
}
