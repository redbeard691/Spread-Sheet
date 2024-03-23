// <copyright file="VariableNode.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ExpressionTree.Nodes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    /// <summary>
    /// Create nodes to hold variables and their values.
    /// </summary>
    public class VariableNode : Node
    {
        private readonly string name = string.Empty;
        private Dictionary<string, double> valuesDict;

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableNode"/> class.
        /// </summary>
        /// <param name="name">Variable Name.</param>
        /// <param name="valueDictionary">ValueDictionary.</param>
        public VariableNode(string name, ref Dictionary<string, double> valueDictionary)
        {
            this.name = name.ToUpper();
            this.valuesDict = valueDictionary;
        }

        /// <summary>
        /// Override Evaluate method.
        /// </summary>
        /// <returns>nothing is returned.</returns>
        public override double Evaluate()
        {
            try
            {
                return this.valuesDict[this.name.ToUpper()];
            }
            catch (Exception e)
            {
                e.ToString();
                this.valuesDict.Add(this.name.ToUpper(), 0);
                return this.Evaluate();
            }
        }
    }
}
