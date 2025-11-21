# ������������ ������ 1 #

## ������� 1.2 ##

### �������� ������ ���������������� �++ � C# �� ������� ������� 1.1 ###

����� ��������: C++ � ������������� ���� ������ ����������, C# �������� �� ��������� .NET � ����������� ������ ����������.


#### 1. ����������� ��������� � ������� ������:
�++:
������������ #include:

    // ��������� ����������
    #include <iostream>      // ����-�����
    #include <vector>        // ��������� vector

    // ���������������� ������������ ����� (�������)
        #include "myclass.h"
        #include "../utils/helper.h"
        #include "config.h"
***
    C#:
    ��������� using � ������������ ����

    // ��������� ����������
        using System;                      // ������� ����
        using System.Collections.Generic; // Generic-���������

    // ���������������� ������������ ����
        using MyProject.Models;
        using MyProject.Utils;


#### 2. ����� ���������:
    � �++ ��� ������ ���������� ������ ���������
```
double CircleSquare()
{
}
int main()
{
}
```
    � C# ����� ���� ��������� ����� ����� ���������:

```
	class Program
{
    static double CircleSquare()
    {
    }

    static void Main(string[] args)
    {
    }
}
```

#### 3. ���������� �������:

    ������� ����������
    C# - ������ ������
```
public int Add(int a, int b) 
{
    return a + b;
}

// ����������� �����
public static void PrintMessage(string message) 
{
    Console.WriteLine(message);
}
```
***
    C++ - ���������� ������� ���������
```
int Add(int a, int b) 
{
    return a + b;
}


// � ������
       
class Calculator 
{
public:
    static void PrintMessage(string message) 
    {
        cout << message << endl;
    }
};
```


#### 4. �������� ���������� �� ������

� �++ �������� �� ������ ������������ & ����� ���� string
```
void ProcessString(const string& input) 
{}
```
***
� �# string �������� ��������� ����� �� ���������, ������ ���� - � ������� ��������� ����� ref
***

#### 5. ���������� �������


C# - �������������� ���������� (GC):
```
    // ������ ����������� ��������� ������
        MyClass obj = new MyClass();
        // �� ����� ���� ������� ������
```
***
C++ - ������ ����������:
```
    // ������������ ��������� ������
        MyClass* obj = new MyClass();
        // ����������� ����� �������
        delete obj;
```
***

#### 6. ������� � ������
- � �# ��� ������� ������� - ���� ������, ������� ��������� � ������ ���������, � ������ ������������ ��� ������(static void ����� �������)
- � C++ ���� ����������� �������

#### 7. Main

� C# - ����������� ����� Main ������ ���� static
```
static void Main(string[] args) 
{  // ���������
}

void Main(string[] args) 
{  // ������ ����������
}
```
***
� C++ �� ����������� static

������������ ���:
� �++ ������ int (��� �������� ��)
```
int main() 
{
    return 0;  // 0 = �����, ��-0 = ������
}
```
***
� C# ������ void, �� ����� ���� int ��� Task
```
static void Main(string[] args) 
{  // ��� ������������� ��������
}

static int Main(string[] args) 
{  // � ����� ��������
    return Environment.ExitCode;
}
```
***