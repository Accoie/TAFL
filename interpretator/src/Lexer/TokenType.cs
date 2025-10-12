public enum TokenType
{
    /// <summary>
    /// Идентификаторы (имена переменных, функций и т.д.)
    /// </summary>
    Identifier,

    /// <summary>
    /// Целочисленные литералы (0, 100, 999)
    /// </summary>
    Integer,

    /// <summary>
    /// Вещественные литералы (3.14, 0.5, 2.0)
    /// </summary>
    Float,

    /// <summary>
    /// Строковые литералы в двойных кавычках
    /// </summary>
    StringLiteral,

    /// <summary>
    /// Оператор сложения.
    /// </summary>
    PlusSign,

    /// <summary>
    /// Оператор вычитания.
    /// </summary>
    MinusSign,

    /// <summary>
    /// Оператор умножения.
    /// </summary>
    MultiplySign,

    /// <summary>
    /// Оператор деления.
    /// </summary>
    DivideSign,

    /// <summary>
    /// Оператор деления по модулю.
    /// </summary>
    ModuloSign,

    /// <summary>
    /// Оператор сравнения равно (==)
    /// </summary>
    Equal,

    /// <summary>
    /// Оператор сравнения не равно (!=)
    /// </summary>
    NotEqual,

    /// <summary>
    /// Оператор сравнения "меньше".
    /// </summary>
    LessThan,

    /// <summary>
    /// Оператор сравнения "меньше или равно".
    /// </summary>
    LessThanOrEqual,

    /// <summary>
    /// Оператор сравнения "больше".
    /// </summary>
    GreaterThan,

    /// <summary>
    /// Оператор сравнения "больше или равно".
    /// </summary>
    GreaterThanOrEqual,

    /// <summary>
    /// Логический оператор ИЛИ (||)
    /// </summary>
    LogicalOr,

    /// <summary>
    /// Логический оператор И (@)
    /// </summary>
    LogicalAnd,

    /// <summary>
    /// Логический оператор НЕ (!)
    /// </summary>
    LogicalNot,

    /// <summary>
    /// Оператор присваивания (=)
    /// </summary>
    Assign,

    /// <summary>
    /// Левая квадратная скобка [
    /// </summary>
    LBracket,

    /// <summary>
    /// Правая квадратная скобка ]
    /// </summary>
    RBracket,

    /// <summary>
    /// Точка с запятой ; (конец инструкции)
    /// </summary>
    Semicolon,

    /// <summary>
    /// Запятая , (разделитель в списках)
    /// </summary>
    Comma,

    /// <summary>
    /// Двоеточие : (разделитель типа)
    /// </summary>
    Colon,

    /// <summary>
    /// Открывающая круглая скобка '('.
    /// </summary>
    LParen,

    /// <summary>
    /// Закрывающая круглая скобка ')'.
    /// </summary>
    RParen,

    /// <summary>
    /// Левая фигурная скобка {
    /// </summary>
    LBrace,

    /// <summary>
    /// Правая фигурная скобка }
    /// </summary>
    RBrace,

    /// <summary>
    /// Комментарии (однострочные ## и многострочные /# ... #/)
    /// </summary>
    Comment,

    /// <summary>
    /// Конец файла.
    /// </summary>
    EndOfFile,

    /// <summary>
    /// Недопустимая лексема.
    /// </summary>
    Error,

    /// <summary>
    /// Ключевое слово НАЧАЛО (начало блока кода)
    /// </summary>
    Begin,

    /// <summary>
    /// Ключевое слово ИСХОД (конец блока кода)
    /// </summary>
    End,

    /// <summary>
    /// Ключевое слово ЧИСЛО (объявление переменной)
    /// </summary>
    Number,

    /// <summary>
    /// Ключевое слово ЦЕС (целочисленный тип)
    /// </summary>
    IntegerType,

    /// <summary>
    /// Ключевое слово ДРОБЬ (число с плавающей точкой)
    /// </summary>
    FloatType,

    /// <summary>
    /// Ключевое слово СЛОВО (строковый тип)
    /// </summary>
    Word,

    /// <summary>
    /// Ключевое слово БУЛЕВО (логический тип)
    /// </summary>
    BooleanType,

    /// <summary>
    /// Ключевое слово ЕЖЕЛИ (начало условного оператора)
    /// </summary>
    If,

    /// <summary>
    /// Ключевое слово СТАЛОБЫТЬ (условие выполнено)
    /// </summary>
    Then,

    /// <summary>
    /// Ключевое слово ИНО (иначе)
    /// </summary>
    Else,

    /// <summary>
    /// Ключевое слово ДЛЯ (начало цикла)
    /// </summary>
    For,

    /// <summary>
    /// Ключевое слово ОТ (начальное значение)
    /// </summary>
    From,

    /// <summary>
    /// Ключевое слово ДО (конечное значение)
    /// </summary>
    To,

    /// <summary>
    /// Ключевое слово ТВОРИ (начало исполняемого блока)
    /// </summary>
    Do,

    /// <summary>
    /// Ключевое слово ПОКУДА (цикл while)
    /// </summary>
    While,

    /// <summary>
    /// Ключевое слово МОЛВИ (вывод на экран)
    /// </summary>
    Output,

    /// <summary>
    /// Ключевое слово ВНЕМЛИ (ввод данных)
    /// </summary>
    Input,

    /// <summary>
    /// Ключевое слово ИСТИНА (логическое значение истина)
    /// </summary>
    True,

    /// <summary>
    /// Ключевое слово ЛОЖЬ (логическое значение ложь)
    /// </summary>
    False,

    /// <summary>
    /// Ключевое слово ВЫЙТИ (выход из цикла)
    /// </summary>
    Break,

    /// <summary>
    /// Ключевое слово ПРОДОЛЖИТЬ (продолжить цикл)
    /// </summary>
    Continue,

    /// <summary>
    /// Ключевое слово ДАРОВАТЬ (вернуть значение функции)
    /// </summary>
    Return,

    /// <summary>
    /// Ключевое слово БУЛЕВО (новая переменная с логическим типом)
    /// </summary>
    Boolean,
}