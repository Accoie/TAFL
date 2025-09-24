# Лабораторная работа 1 #

## Задание 1.2 ##

### Различие языков программирования С++ и C# на примере задания 1.1 ###

Общие различия: C++ — компилируемый язык общего назначения, C# работает на платформе .NET с управляемой средой выполнения.


#### 1. Подключение библиотек и внешних файлов:
С++:
Используется #include:

    // Системные библиотеки
    #include <iostream>      // Ввод-вывод
    #include <vector>        // Контейнер vector

***
    // Пользовательские заголовочные файлы (кавычки)
        #include "myclass.h"
        #include "../utils/helper.h"
        #include "config.h"
***
    C#:
    Директивы using и пространства имен
***
    // Системные библиотеки
        using System;                      // Базовые типы
        using System.Collections.Generic; // Generic-коллекции
***
    // Пользовательские пространства имен
        using MyProject.Models;
        using MyProject.Utils;


#### 2. Класс программы:
    В С++ нет явного объявления класса программы
    ```
    double CircleSquare()
    {
    }
    int main()
    {
    }
    ```
	    В C# нужно явно указывать класс нашей программы:

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

#### 3. Объявление функций:


    Базовое объявление

        // C# - внутри класса
        ```
            public int Add(int a, int b) 
            {
                return a + b;
            }
        ```
***
        ```
        // Статический метод
        public static void PrintMessage(string message) 
        {
            Console.WriteLine(message);
        }
        ```
***
        /// C++ - глобальные функции разрешены
        ```
        int Add(int a, int b) 
        {
            return a + b;
        }
        ```
***
        // В классе
        ```
        class Calculator 
        {
        public:
            static void PrintMessage(string message) 
            {
                cout << message << endl;
            }
        };
        ```


#### 4. Передача параметров по ссылке

    В С++ передача по ссылке обозначается & после типа string
        void ProcessString(const string& input) 
        {}
***
    В С# string является ссылочным типом по умолчанию, другие типы - с помощью ключевого слова ref
***

#### 5. Управление памятью


    C# - автоматическое управление (GC):

    // Память управляется сборщиком мусора
        MyClass obj = new MyClass();
        // Не нужно явно удалять объект
***
    C++ - ручное управление:

    // Динамическое выделение памяти
        MyClass* obj = new MyClass();
        // Обязательно нужно удалить
        delete obj;
***

#### 6. Функции и методы
    - В С# нет простых функций - есть методы, которые относятся к классу программы, и всегда обозначаются как методы(static void перед методом)
    - В C++ есть независимые функции

#### 7. Main

 В C# - ОБЯЗАТЕЛЬНО метод Main должен быть static
static void Main(string[] args) 
{  // Правильно
}

void Main(string[] args) 
{  // ОШИБКА компиляции
}
***
В C++ не обязательно static

Возвращаемый тип:
В С++ обычно int (код возврата ОС)
int main() 
{
    return 0;  // 0 = успех, не-0 = ошибка
}
***
В C# обычно void, но может быть int или Task
static void Main(string[] args) {  // Нет возвращаемого значения
}

static int Main(string[] args) {  // С кодом возврата
    return Environment.ExitCode;
}
***