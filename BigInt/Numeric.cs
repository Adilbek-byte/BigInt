using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace BigIntTask;
// Abstract base class for numeric types
public abstract class Numeric<T>: INumeric<T> where T : Numeric<T>
{
    // Constants for characters
    private const char MINUS_CHAR = '-';
    private const char PLUS_CHAR = '+';

    // List to store individual digits of the number
    protected List<int> numberDigits = new List<int>();

    // Flag to indicate negativity
    protected bool negative;

    // String representation of the number
    protected string? stringNumber;

    // Constructor to initialize the numeric object
    public Numeric(string number)
    {
        // handling cases of incorrect inputs 
        try
        {
            // Handling cases where the input is an empty string
            if (number.Equals(""))
            {
                stringNumber = "0";
                numberDigits.Add(0);
            }
            else
            {
                // Check for negative sign and remove leading zeros
                char firstChar = number[0];
                if (firstChar == MINUS_CHAR || firstChar == PLUS_CHAR)
                {
                    if (firstChar == MINUS_CHAR)
                        negative = true;

                    number = number.Substring(1);
                }

                number = System.Text.RegularExpressions.Regex.Replace(number, "^0+(?!$)", "");
                stringNumber = number;

                for (int index = 0; index < number.Length; index++)
                {
                    int curDigNumericVal = int.Parse(number[index].ToString());

                    if (curDigNumericVal == -1)
                        throw new ArgumentException();

                    numberDigits.Add(curDigNumericVal);
                }
            }
        }
        // Handle the exception appropriately, display a message
        catch (Exception ex)
        {
            Console.WriteLine($"Error in input: {ex.Message}");
            //rethrow the exception or handle it in a way that fits your application.
            throw;
        }
    }
    // Method to check if the number is negative
    public bool IsNegative()
    {
        return negative;
    }

    // Method to flip the negativity of the number
    public void FlipNegativity()
    {
        if (stringNumber == "0")
            return;

        negative = !negative;
        if (stringNumber[0] == MINUS_CHAR)
        {
            stringNumber = stringNumber.Substring(1);
        }
        else
        {
            stringNumber = MINUS_CHAR + stringNumber;
        }
    }
    // Method to perform addition
    public T Plus(T other)
    {
        if (negative && !other.IsNegative())
        {
            return other.Minus((T)this);
        }

        if (other.IsNegative())
        {
            return Minus(Negate(other));
        }

        List<int> longerNumber, shorterNumber;
        if (numberDigits.Count >= other.numberDigits.Count)
        {
            longerNumber = numberDigits;
            shorterNumber = other.numberDigits;
        }
        else
        {
            longerNumber = other.numberDigits;
            shorterNumber = numberDigits;
        }

        int lengthsDifferences = longerNumber.Count - shorterNumber.Count;
        System.Text.StringBuilder resultString = new System.Text.StringBuilder();
        int carry = 0;

        for (int index = shorterNumber.Count - 1; index >= 0; index--)
        {
            int shorterNumberDigit = shorterNumber[index];
            int longerNumberDigit = longerNumber[index + lengthsDifferences];

            int newDigit = shorterNumberDigit + longerNumberDigit + carry;

            carry = newDigit / 10;
            newDigit = newDigit % 10;

            resultString.Append(newDigit);
        }

        for (int index = lengthsDifferences - 1; index >= 0; index--)
        {
            int currDig = longerNumber[index];

            if (currDig + carry == 10)
            {
                resultString.Append(0);
                carry = 1;
            }
            else
            {
                resultString.Append(currDig + carry);
                carry = 0;
            }
        }

        if (carry > 0)
            resultString.Append(carry);

        return CreateInstance(new string(resultString.ToString().ToCharArray().Reverse().ToArray()));
    }

    // Method to perform subtraction
    public T Minus(T other)
    {
        if (other.IsNegative())
        {
            return Plus(Negate(other));
        }

        if (this.CompareTo(other) < 0)
        {
            T result = other.Minus((T)this);
            return Negate(result);
        }

        int lengthsDifferences = numberDigits.Count - other.numberDigits.Count;
        System.Text.StringBuilder resultString = new System.Text.StringBuilder();
        int carry = 0;

        for (int index = other.numberDigits.Count - 1; index >= 0; index--)
        {
            int biggerNumDig = numberDigits[index + lengthsDifferences] - carry;
            int smallerNumDig = other.numberDigits[index];

            carry = 0;

            if (biggerNumDig < smallerNumDig)
            {
                carry = 1;
                biggerNumDig += 10;
            }

            resultString.Append(biggerNumDig - smallerNumDig);
        }

        for (int index = lengthsDifferences - 1; index >= 0; index--)
        {
            int currDig = numberDigits[index];

            if (carry > currDig)
            {
                resultString.Append(currDig + 10 - carry);
                carry = 1;
            }
            else
            {
                resultString.Append(currDig - carry);
                carry = 0;
            }
        }

        return CreateInstance(new string(resultString.ToString().ToCharArray().Reverse().ToArray()));
    }

    // Method to perform multiplication
    public T Multiply(T other)
    {
        T finalResult = CreateInstance("0");

        for (int i = other.numberDigits.Count - 1; i >= 0; i--)
        {
            int currentOtherNumDigit = other.numberDigits[i];

            T partialResult = CreateInstance("0");
            T currentUnit = CreateInstance(this.ToString());

            for (int j = 0; j < currentOtherNumDigit; j++)
            {
                partialResult = partialResult.Plus(currentUnit);
            }

            for (int k = 0; k < other.numberDigits.Count - 1 - i; k++)
            {
                partialResult.MultiplyByTen();
            }

            finalResult = finalResult.Plus(partialResult);
        }

        // Check if need to flip negativity
        if ((other.IsNegative() && !IsNegative()) || (IsNegative() && !other.IsNegative()))
            finalResult = Negate(finalResult);

        return finalResult;
    }


    // Method to perform division
    public T Divide(T other)
    {
        T result;
        T tempNumber;
        T currentNumber = (T)this;

        if (IsBigIntZero(other))
            throw new DivideByZeroException("it's not allowed to divide by zero!");

        if (other.IsNegative() && !IsNegative())
        {
            result = Divide(Negate(other));
            return Negate(result);
        }
        else if (!other.IsNegative() && IsNegative())
        {
            result = CreateInstance(stringNumber!).Divide(other);
            return Negate(result);
        }

        int compareResult = this.CompareTo(other);
        if (compareResult == 0)
            return CreateInstance("1");
        else if (compareResult < 0)
            return CreateInstance("0");

        result = CreateInstance("0");
        tempNumber = CreateInstance("0");

        while (tempNumber.CompareTo(currentNumber) <= 0)
        {
            tempNumber = tempNumber.Plus(other);
            result = result.Plus(CreateInstance("1"));
        }

        return result.Minus(CreateInstance("1"));
    }

    // Method to perform modulo operation
    public T Mod(T other)
    {
        if (IsBigIntZero(other))
            throw new ArithmeticException("Cannot perform modulo operation with zero divisor. ");
        T quotient = Divide(other);
        T product = other.Multiply(quotient);
        T remainder = Minus(product);

        while(remainder.CompareTo(other) >= 0)
        {
            remainder = remainder.Minus(other);
        }
        if (remainder.IsNegative())
            remainder = remainder.Plus(other);

        return remainder;
    }
    
    
    // Method to compare two numeric values 
    public int CompareTo(T other)
    {
        if (IsNegative() && !other.IsNegative())
            return -1;
        else if (!IsNegative() && other.IsNegative())
            return 1;
        else if (IsNegative())
        {
            if (numberDigits.Count > other.numberDigits.Count)
                return -1;
            else if (numberDigits.Count < other.numberDigits.Count)
                return 1;

            for (int index = 0; index < numberDigits.Count; index++)
            {
                if (numberDigits[index] > other.numberDigits[index])
                    return -1;
                else if (numberDigits[index] < other.numberDigits[index])
                    return 1;
            }

            return 0;
        }

        if (numberDigits.Count > other.numberDigits.Count)
            return 1;
        else if (numberDigits.Count < other.numberDigits.Count)
            return -1;

        for (int index = 0; index < numberDigits.Count; index++)
        {
            if (numberDigits[index] > other.numberDigits[index])
                return 1;
            else if (numberDigits[index] < other.numberDigits[index])
                return -1;
        }

        return 0;
    }

    //Method to override Equals method
    public override bool Equals(object? obj)
    {
        if (this == obj)
            return true;

        if (obj == null)
            return false;

        if (GetType() != obj.GetType())
            return false;

        T other = (T)obj;

        return other.ToString().Equals(stringNumber);
    }

    // Method to override ToString method
    public override string ToString()
    {
        return stringNumber!;
    }

   // Abstract method to create an instance of the derived class
    protected abstract T CreateInstance(string number);

    // Abstract method to negate a numeric value
    protected abstract T Negate(T number);

    // Method to check if a BigInt is zero
    private bool IsBigIntZero(T number)
    {
        return number.stringNumber!.Replace("0", "").Equals("");
    }

    // Method to multiply the number by a unit
    private T MultiplyUnit(int majorUnits)
    {
        string majorUnitsString = majorUnits.ToString();
        string newNumber = majorUnitsString + stringNumber?.Substring(1);

        return CreateInstance(new string(newNumber.ToCharArray().Reverse().ToArray()));
    }

    // Method to multiply the number by ten
    private void MultiplyByTen()
    {
        numberDigits.Add(0);
        stringNumber += '0';
    }
}



