using System;

class Program
{
    static string ReverseString(string input)
    {
        char[] result = new char[input.Length];

        for (int i = 0; i < input.Length; i++)
        {
            result[i] = input[input.Length - 1 - i];
        }

        return new string(result);
    }

    static void Main(string[] args)
    {
        Console.WriteLine("Введите строку для переворота:");
        string input = Console.ReadLine();
        string reversed = ReverseString(input);
        Console.WriteLine($"Перевернутая строка: {reversed}");
    }
}