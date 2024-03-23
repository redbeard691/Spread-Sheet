using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpressionTree;
using ExpressionTree.Nodes;
using SpreadSheet_Engine;

namespace Test_Unit_SpreadSheet_Engine
{
    internal class ExpressTreeTests
    {

        [Test]
        public void ShuntingYardAlgorithm_ShouldConvertInfixToPostfix()
        {
            // Arrange
            
            string infixExpression = "2+3*(4-1)";

            ExpressionTreeClass shuntingYard = new ExpressionTreeClass(infixExpression);
            Queue<string> expectedPostfixExpression = new Queue<string>(new[] { "2", "3", "4", "1", "-", "*", "+" });

            // Act
            Queue<string> result = shuntingYard.ShuntingYardAlgorithm(infixExpression);

            // Assert
            CollectionAssert.AreEqual(expectedPostfixExpression, result.ToArray());
        }

        [Test]
        public void CreateOperatorNode_ShouldReturnValidOperatorNode()
        {
            // Arrange
            OperatorNodeFactory operatorNodeFactory = new OperatorNodeFactory();
            char validOperator = '+';

            // Act
            OperatorNode? result = operatorNodeFactory.CreateOperatorNode(validOperator);

            // Assert
            Assert.IsNotNull(result, "OperatorNode should not be null.");
            // You can further assert the type or properties of the returned OperatorNode if needed.
        }

        [Test]
        public void CreateOperatorNode_ShouldReturnNullForInvalidOperator()
        {
            // Arrange
            OperatorNodeFactory operatorNodeFactory = new OperatorNodeFactory();
            char invalidOperator = '$'; // Assuming '$' is not a valid operator in your context

            // Act
            OperatorNode? result = operatorNodeFactory.CreateOperatorNode(invalidOperator);

            // Assert
            Assert.IsNull(result, "Invalid OperatorNode should be null.");
        }

    }
}
