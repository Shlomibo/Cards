using System;
using System.Text;

namespace ConsoleUtils.Output;

internal class StringConsoleOutput(string? str, bool newLine = false) : IConsoleOutput
{
    public void Print(StringBuilder stringBuilder)
    {
        if (newLine)
        {
            stringBuilder.AppendLine(str);
        }
        else
        {
            stringBuilder.Append(str);
        }
    }

    public static implicit operator StringConsoleOutput(string output) => new(output);
}
