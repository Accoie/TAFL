using System;

class Program
{
    static void BubbleSort(int[] arr)
    {
        int n = arr.Length;
        for (int i = 0; i < n - 1; i++)
        {
            for (int j = 0; j < n - i - 1; j++)
            {
                if (arr[j] > arr[j + 1])
                {
                    int temp = arr[j];
                    arr[j] = arr[j + 1];
                    arr[j + 1] = temp;
                }
            }
        }
    }

    static void Main(string[] args)
    {
        Console.WriteLine("Введите числа через пробел:");
        string input = Console.ReadLine();

        if (string.IsNullOrEmpty(input))
        {
            Console.WriteLine("Вы ввели пустую строку!");
            return;
        }

        string[] numbersStr = input.Split(' ');
        int[] numbers = new int[numbersStr.Length];

        try
        {
            for (int i = 0; i < numbersStr.Length; i++)
            {
                numbers[i] = int.Parse(numbersStr[i]);
            }

            BubbleSort(numbers);

            Console.WriteLine("Отсортированные числа: " + string.Join(" ", numbers));
        }
        catch (FormatException)
        {
            Console.WriteLine("Ошибка: введите только целые числа!");
        }
    }
}