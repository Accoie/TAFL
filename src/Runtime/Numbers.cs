public static class Numbers
{
    // Допустимая абсолютная погрешность сравнения чисел с плавающей точкой.
    public const decimal Tolerance = 0.001m;

    public static bool AreEqual(decimal a, decimal b)
    {
        return Math.Abs(a - b) < Tolerance;
    }

    public static bool IsLessThan(decimal a, decimal b)
    {
        return a < b && !AreEqual(a, b);
    }

    public static bool IsLessThanOrEqual(decimal a, decimal b)
    {
        return a < b || AreEqual(a, b);
    }

    public static bool IsGreaterThan(decimal a, decimal b)
    {
        return a > b && !AreEqual(a, b);
    }

    public static bool IsGreaterThanOrEqual(decimal a, decimal b)
    {
        return a > b || AreEqual(a, b);
    }
}