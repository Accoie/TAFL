# Сравнение лексических структур C# и C++
## Элементы структурного программирования:
### блоки кода
идентичные
```
{
    int x = 10;
    // код
}
```
------------
### ветвления
В C# break обязателен в каждом case, так же можно задавать доп условия через condition
```
switch (variable) {
    case 1:
        // код
        break;
    case 2 when condition:
        // код
        break;
    default:
        // код
        break; // обязателен
}
```
B С++:
```

// switch
switch (variable) {
    case 1:
        // код
    case 2:
        // код
    default:
        // код
}
```
--------------
### циклы
Отличается синтаксис foreach
В C#:
```
foreach (var item in collection) {
    // код
}
```
В C++:
```
for (auto& item : collection) {
    // код
}
```
## Средства для ввода/вывода и другие встроенные функции
C++
```
#include <iostream>
using namespace std;

// вывод
cout << "Hello" << variable;
printf("Value: %d", x);

// ввод
cin >> variable;
scanf("%d", &x);
```
C#
```
using System;

// вывод
Console.WriteLine("Hello " + variable);
Console.WriteLine($"Value: {variable}"); // интерполяция

// ввод
string input = Console.ReadLine();
```
Различия: C# использует класс Console, есть интерполяция строк

## Пользовательские функции, параметры и возвращаемые значения
C++
```
// void функция
void printMessage(const string& message) {
    cout << message;
}
```
C#
```
// void метод
void PrintMessage(string message) {
    Console.WriteLine(message);
}

// опциональные параметры
void Method(int required, string optional = "default") {
    // код
}
```
Различия: C# поддерживает опциональные параметры со значениями по умолчанию

## Встроенные типы данных для целых чисел, числе с плавающей точкой и строк

C++
```
// целые
int, short, long, long long
unsigned int, size_t

// с плавающей точкой
float, double, long double

// строки
std::string, const char*
bool
```
C#
```
// целые
int, short, long
byte, sbyte
uint, ushort, ulong

// с плавающей точкой
float, double, decimal

// строки
string, char
bool
```
Различия: C# имеет беззнаковые типы с префиксом 'u', тип decimal для финансовых расчетов

## Выражения с операторами
### арифметические операторы
Одинаковые: + - * / % ++ --
### логические операторы
C++: 
```
&& || !
```
C#: 
```
&& || ! (дополнительно & | для побитовых операций)
```
Различия: В C# операторы & и | не являются короткозамкнутыми
### операторы сравнения
Одинаковые: 
```
== != < > <= >=
```
## Пользовательские составные типы данных
### массивы
C++
```
// статический
int arr[10];

// динамический
int* dynamicArr = new int[10];
delete[] dynamicArr;

// vector (рекомендуется)
#include <vector>
std::vector<int> vec = {1, 2, 3};
```
C#
```
// одномерный
int[] arr = new int[10];
int[] arr2 = {1, 2, 3};

// многомерный
int[,] matrix = new int[3,3];
int[][] jagged = new int[3][];
```
Различия: C# имеет встроенную поддержку массивов с свойством Length
### структуры (записи)
C++
```
struct Point {
    int x;
    int y;
    
    // могут содержать методы
    void print() {
        cout << x << ", " << y;
    }
};

// использование
Point p = {10, 20};
```
C#
```
struct Point {
    public int X;
    public int Y;
    
    // могут содержать методы
    public void Print() {
        Console.WriteLine($"{X}, {Y}");
    }
}

// record (C# 9+)
record Person(string Name, int Age);
```
Различия: C# структуры - типы значений, C++ - зависит от использования. C# имеет records для неизменяемых данных.