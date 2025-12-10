using Runtime;

namespace Ast;

public class Parameter(string name, Runtime.ValueType type)
{
    public string Name { get; set; } = name;

    public Runtime.ValueType Type { get; set; } = type;
}