using System;
using System.Text;

namespace ConsoleUtils.Output;

internal abstract class OutputModBase(
    IConsoleOutput output,
    string prefix,
    string suffix) : IConsoleOutput
{
    public void Print(StringBuilder stringBuilder)
    {
        stringBuilder.Append(prefix);
        output.Print(stringBuilder);
        stringBuilder.Append(suffix);
    }
}
