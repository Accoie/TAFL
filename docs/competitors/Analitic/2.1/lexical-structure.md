# Сравнение лексических структур C# и C++
## Элементы структурного программирования:
### Блоки кода
идентичные
```
{
    int x = 10;
    // код
}
```
------------
### Ветвления
В C# break обязателен в каждом case, так же можно задавать доп условия через condition
```
switch (variable) 
{
    case 1:
        // код
        break;
    case 2 when condition:
        // код
        break;
    default:
        // код
        break; 
}
```
B С++:
```

// switch
switch (variable) 
{
    case 1:
        // код
    case 2:
        // код
    default:
        // код
}
```
--------------
### Циклы
Отличается синтаксис цикла for
В C#: называется foreach и в выражении используется "in" для перечисления элементов 
```
foreach (var item in collection) 
{
    // код
}
```
В C++:
```
for (auto& item : collection) 
{
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
scanf("%d", &x);   //%d - целое число
```
C#
```
using System;

// вывод
Console.WriteLine("Hello " + variable);
Console.WriteLine($"Value: {variable}");

// ввод
string input = Console.ReadLine();
```
Различия: C# использует класс Console, есть интерполяция строк `($"Value: {variable}")`. В c++ используются потоки ввода-вывода через токены `<<` и `>>`

## Пользовательские функции, параметры и возвращаемые значения
C++
```
void printMessage(const string& message) 
{
    cout << message;
}
```
C#
```
void PrintMessage(string message) 
{
    Console.WriteLine(message);
}

void Method(int required, string optional = "default") 
{
    // код
}
```
Различия: C# поддерживает опциональные параметры со значениями по умолчанию, так же в C# тип стринг является ссылочным по умолчанию

## Встроенные типы данных для целых чисел, числе с плавающей точкой и строк

C++
```
//целые
int, short, long, long long
unsigned int, size_t

// с плавающей точкой
float, double, long double

// строки
std::string, const char*

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
### Арифметические операторы
Одинаковые: `+ - * / % ++ --`
### Логические операторы
C++: 
```
&& || !
```
C#: 
```
&& || !
```
Различия: В C# операторы & и | не являются короткозамкнутыми
### Операторы сравнения
Одинаковые: 
```
== != < > <= >=
```
## Пользовательские составные типы данных
### Массивы
C++
```
// статический
int arr[10];

// динамический
int* dynamicArr = new int[10];
delete[] dynamicArr;

// vector
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
Различия: Отличаются объявления массива. 
C# использует квадратные скобки с типом, а C++ - с именем переменной. 
C# имеет встроенную поддержку массивов с свойством Length
### Структуры (записи)
C++
```
struct Point 
{
    int x;
    int y;
    
    void print() 
    {
        cout << x << ", " << y;
    }
};

// использование
Point p = {10, 20};
```
C#
```
struct Point 
{
    public int X;
    public int Y;
    
    public void Print() 
    {
        Console.WriteLine($"{X}, {Y}");
    }
}

record Person(string Name, int Age);
```
Различия: C# структуры - типы значений, C++ - зависит от использования. C# имеет records для неизменяемых данных.
### Ключевые слова

Уникальные для C++:
```
// Управление памятью
new, delete, new[], delete[]

// Модификаторы типов
const, volatile, mutable, constexpr (C++11)

// Шаблоны и наследование
template, typename, friend, virtual, override, final (C++11)

// Пространства имен
namespace, using

// Препроцессор
#define, #include, #ifdef, #pragma
```

Уникальные для C#:
```
// Модификаторы доступа и классов
public, private, protected, internal, sealed, abstract

// Свойства и события
get, set, value, event, delegate

// Управление памятью и исключения
using, try, catch, finally, throw, checked, unchecked

// Типы и преобразования
class, struct, enum, interface, is, as, explicit, implicit

// Современные возможности
var, dynamic, async, await, nameof, yield
```

### Знаки экранирования
Общие escape-последовательности:
```
\n - новая строка

\t - табуляция

\\ - обратная косая черта

\" - двойная кавычка

\' - одинарная кавычка
```
