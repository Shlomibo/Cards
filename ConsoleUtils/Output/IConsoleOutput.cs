using System;
using System.Text;

namespace ConsoleUtils.Output;

public interface IConsoleOutput
{
    void Print(StringBuilder stringBuilder);

    static virtual IConsoleOutput operator +(IConsoleOutput left, IConsoleOutput right) =>
        new MultiOutput([left, right]);
}
