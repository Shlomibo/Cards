using System;
using System.Text;

namespace ConsoleUtils.Output;

internal abstract class OutputModBase(
    ConsoleOutput output,
    string prefix,
    string suffix) : ConsoleOutput
{
    public sealed override void Print(StringBuilder stringBuilder)
    {
        stringBuilder.Append(prefix);
        output.Print(stringBuilder);
        stringBuilder.Append(suffix);
    }
}
