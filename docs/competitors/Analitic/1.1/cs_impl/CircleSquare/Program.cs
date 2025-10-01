using System;
class Program
{
    static double CircleSquare()
    {
        double radius = 12;
        double square = 3.14 * radius * radius;
        return square;
    }

    static void Main(string[] args)
    {
        double answer = CircleSquare();
        Console.WriteLine("Square of circle: " + answer); 
    }
}