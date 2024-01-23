using BigIntTask;
public class Program
{
    private static Scanner scanner = new Scanner();
    private static void Main(string[] args)
    {
        while (true)
        {
            // declaration of two BigInt variables to store BigIntegers 
            BigInt firstNumber;
            BigInt secondNumber;

            Console.WriteLine("Enter first number: ");
            firstNumber = InputBigIntNumber();

            Console.WriteLine("Enter second number: ");
            secondNumber = InputBigIntNumber();



            Console.WriteLine("The result of plus + is: " + firstNumber.Plus(secondNumber));
            Console.WriteLine("The result of minus - is: " + firstNumber.Minus(secondNumber));
            Console.WriteLine("The result of multiply * is: " + firstNumber.Multiply(secondNumber));
            Console.WriteLine("The result of mod % is: " + firstNumber.Mod(secondNumber));
            
            Console.WriteLine("The result of divide is / :" + firstNumber.Divide(secondNumber));
            
           
            Console.ReadKey();
            Console.Clear();
        }
    }
    // Method to input a BigInt number
    private static BigInt InputBigIntNumber()
    {
        string str = scanner.NextLine();
        
        while (true)
        {
            try
            {
                return new BigInt(str);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Invalid number, please try again: ");
            }
        }
    }


    // Scanner class for input
    public class Scanner
    {
        public string NextLine()
        {
            return Console.ReadLine() ?? string.Empty;
        }
    }
}