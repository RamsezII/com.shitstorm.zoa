using System;
using System.Linq.Expressions;

public static partial class Util_zoa
{
    public static bool CanBeAssignedTo(this Type a, in Type b)
    {
        if (b.IsAssignableFrom(a))
            return true;

        if (a == typeof(int) && b == typeof(float))
            return true;

        try
        {
            var param = Expression.Parameter(a, "a");
            Expression.Convert(param, b);
            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    public static bool TryAssign<T>(this object a, out T b)
    {
        try
        {
            b = (T)a;
            return true;
        }
        catch (InvalidOperationException)
        {
            b = default;
            return false;
        }
    }
}