# ��������� ����������� �������� C# � C++
## �������� ������������ ����������������:
### ����� ����
����������
```
{
    int x = 10;
    // ���
}
```
------------
### ���������
� C# break ���������� � ������ case, ��� �� ����� �������� ��� ������� ����� condition
```
switch (variable) 
{
    case 1:
        // ���
        break;
    case 2 when condition:
        // ���
        break;
    default:
        // ���
        break; 
}
```
B �++:
```

// switch
switch (variable) 
{
    case 1:
        // ���
    case 2:
        // ���
    default:
        // ���
}
```
--------------
### �����
���������� ��������� ����� for
� C#: ���������� foreach � � ��������� ������������ "in" ��� ������������ ��������� 
```
foreach (var item in collection) 
{
    // ���
}
```
� C++:
```
for (auto& item : collection) 
{
    // ���
}
```
## �������� ��� �����/������ � ������ ���������� �������
C++
```
#include <iostream>
using namespace std;

// �����
cout << "Hello" << variable;
printf("Value: %d", x);

// ����
cin >> variable;
scanf("%d", &x);   //%d - ����� �����
```
C#
```
using System;

// �����
Console.WriteLine("Hello " + variable);
Console.WriteLine($"Value: {variable}");

// ����
string input = Console.ReadLine();
```
��������: C# ���������� ����� Console, ���� ������������ ����� `($"Value: {variable}")`. � c++ ������������ ������ �����-������ ����� ������ `<<` � `>>`

## ���������������� �������, ��������� � ������������ ��������
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
    // ���
}
```
��������: C# ������������ ������������ ��������� �� ���������� �� ���������, ��� �� � C# ��� ������ �������� ��������� �� ���������

## ���������� ���� ������ ��� ����� �����, ����� � ��������� ������ � �����

C++
```
//�����
int, short, long, long long
unsigned int, size_t

// � ��������� ������
float, double, long double

// ������
std::string, const char*

```
C#
```
// �����
int, short, long
byte, sbyte
uint, ushort, ulong

// � ��������� ������
float, double, decimal

// ������
string, char
bool
```
��������: C# ����� ����������� ���� � ��������� 'u', ��� decimal ��� ���������� ��������

## ��������� � �����������
### �������������� ���������
����������: `+ - * / % ++ --`
### ���������� ���������
C++: 
```
&& || !
```
C#: 
```
&& || !
```
��������: � C# ��������� & � | �� �������� �����������������
### ��������� ���������
����������: 
```
== != < > <= >=
```
## ���������������� ��������� ���� ������
### �������
C++
```
// �����������
int arr[10];

// ������������
int* dynamicArr = new int[10];
delete[] dynamicArr;

// vector
#include <vector>
std::vector<int> vec = {1, 2, 3};
```
C#
```
// ����������
int[] arr = new int[10];
int[] arr2 = {1, 2, 3};

// �����������
int[,] matrix = new int[3,3];
int[][] jagged = new int[3][];
```
��������: ���������� ���������� �������. 
C# ���������� ���������� ������ � �����, � C++ - � ������ ����������. 
C# ����� ���������� ��������� �������� � ��������� Length
### ��������� (������)
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

// �������������
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
��������: C# ��������� - ���� ��������, C++ - ������� �� �������������. C# ����� records ��� ������������ ������.
### �������� �����

���������� ��� C++:
```
// ���������� �������
new, delete, new[], delete[]

// ������������ �����
const, volatile, mutable, constexpr (C++11)

// ������� � ������������
template, typename, friend, virtual, override, final (C++11)

// ������������ ����
namespace, using

// ������������
#define, #include, #ifdef, #pragma
```

���������� ��� C#:
```
// ������������ ������� � �������
public, private, protected, internal, sealed, abstract

// �������� � �������
get, set, value, event, delegate

// ���������� ������� � ����������
using, try, catch, finally, throw, checked, unchecked

// ���� � ��������������
class, struct, enum, interface, is, as, explicit, implicit

// ����������� �����������
var, dynamic, async, await, nameof, yield
```

### ����� �������������
����� escape-������������������:
```
\n - ����� ������

\t - ���������

\\ - �������� ����� �����

\" - ������� �������

\' - ��������� �������
```
