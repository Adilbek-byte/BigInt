using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigIntTask;

// Interface defining basic numeric operations
public interface INumeric<T>
{
    T Plus(T other);
    T Minus(T other);
    T Multiply(T other);
    T Mod(T other);
    T Divide(T other);
    int CompareTo(T other);
}
