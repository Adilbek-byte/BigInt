using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigIntTask;



// Concrete class representing BigInt
public class BigInt : Numeric<BigInt>
{
    // Consturctor for BigInt
    public BigInt(string number) : base(number) { }

    // Method to create an instance of BigInt
    protected override BigInt CreateInstance(string number)
    {
        return new BigInt(number);
    }

    // Method to negate a BigInt
    protected override BigInt Negate(BigInt number)
    {
        BigInt negated = new BigInt(number.ToString());
        negated.FlipNegativity();
        return negated;
    }
    


}

