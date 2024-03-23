namespace ExpressionTree
{
    class Demo
    {
        static void Main()
        {
            int userInput = -1;
            string expression = "A+B";
            ExpressionTreeClass myExpression = new(expression);

            while (userInput != 4)
            {
                Console.WriteLine("Menu (current expression = " + expression + ")");
                Console.WriteLine("1 - Enter a new expression.");
                Console.WriteLine("2 - Set a variable value.");
                Console.WriteLine("3 - Evaluate tree.");
                Console.WriteLine("4 - QUIT");

                userInput = Convert.ToInt32(Console.ReadLine());

                Console.Clear();

                switch (userInput)
                {
                    case 1:
                        Console.WriteLine("Enter new expression: ");
                        expression = Console.ReadLine();
                        myExpression = new ExpressionTreeClass(expression);
                        break;
                    case 2:
                        Console.WriteLine("Enter variable to set:");
                        string var = Console.ReadLine();
                        Console.WriteLine("Enter value to set " + var + " to:");
                        double val = Convert.ToDouble(Console.ReadLine());
                        myExpression.SetVariable(var, val);
                        break;
                    case 3:
                        Console.WriteLine(myExpression.Evaluate().ToString());
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
