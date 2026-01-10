using System;
using System.Text;

namespace ConsoleUtils.Output;

internal sealed class EmptyOutput : ConsoleOutput
{
    public static EmptyOutput Instance { get; } = new();

    public override void Print(StringBuilder stringBuilder)
    {
    }
}
