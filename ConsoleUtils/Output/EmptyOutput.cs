using System;
using System.Text;

namespace ConsoleUtils.Output;

internal sealed class EmptyOutput : ConsoleOutput
{
    public override void Print(StringBuilder stringBuilder)
    {
    }
}
