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
switch (variable) {
    case 1:
        // ���
        break;
    case 2 when condition:
        // ���
        break;
    default:
        // ���
        break; // ����������
}
```
B �++:
```

// switch
switch (variable) {
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
���������� ��������� foreach
� C#:
```
foreach (var item in collection) {
    // ���
}
```
� C++:
```
for (auto& item : collection) {
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
scanf("%d", &x);
```
C#
```
using System;

// �����
Console.WriteLine("Hello " + variable);
Console.WriteLine($"Value: {variable}"); // ������������

// ����
string input = Console.ReadLine();
```
��������: C# ���������� ����� Console, ���� ������������ �����

## ���������������� �������, ��������� � ������������ ��������
C++
```
// void �������
void printMessage(const string& message) {
    cout << message;
}
```
C#
```
// void �����
void PrintMessage(string message) {
    Console.WriteLine(message);
}

// ������������ ���������
void Method(int required, string optional = "default") {
    // ���
}
```
��������: C# ������������ ������������ ��������� �� ���������� �� ���������

## ���������� ���� ������ ��� ����� �����, ����� � ��������� ������ � �����

C++
```
// �����
int, short, long, long long
unsigned int, size_t

// � ��������� ������
float, double, long double

// ������
std::string, const char*
bool
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
����������: + - * / % ++ --
### ���������� ���������
C++: 
```
&& || !
```
C#: 
```
&& || ! (������������� & | ��� ��������� ��������)
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

// vector (�������������)
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
��������: C# ����� ���������� ��������� �������� � ��������� Length
### ��������� (������)
C++
```
struct Point {
    int x;
    int y;
    
    // ����� ��������� ������
    void print() {
        cout << x << ", " << y;
    }
};

// �������������
Point p = {10, 20};
```
C#
```
struct Point {
    public int X;
    public int Y;
    
    // ����� ��������� ������
    public void Print() {
        Console.WriteLine($"{X}, {Y}");
    }
}

// record (C# 9+)
record Person(string Name, int Age);
```
��������: C# ��������� - ���� ��������, C++ - ������� �� �������������. C# ����� records ��� ������������ ������.