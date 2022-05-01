using System.Linq;
using UnityEngine;

public static class MathFunc
{
    private static int Factorial(int x)
    {
        return x == 0 ? 1 : x * Factorial(x - 1);
    }

    public static int Combine(int all, int chosen)
    {
        return Factorial(all) / Factorial(chosen) / Factorial(all - chosen);
    }

    public static int[] NextPtr(int[] ptr, int max)
    {
        ptr[^1]++;
        if (ptr[^1] == max)
        {
            int[] sptr = ptr.Take(ptr.Length - 1).ToArray();
            sptr = NextPtr(sptr, max - 1);
            return sptr.Concat(new int[1] { sptr[^1] + 1 }).ToArray();
        }
        return ptr;
    }
}
