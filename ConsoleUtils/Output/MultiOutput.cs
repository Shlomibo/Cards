using System;
using System.Text;

namespace ConsoleUtils.Output;

internal sealed class MultiOutput(IEnumerable<ConsoleOutput> outputs, bool resetStylesOnEnd = false)
    : ConsoleOutput
{
    public override void Print(StringBuilder stringBuilder)
    {
        foreach (var output in Normalized())
        {
            output.Print(stringBuilder);
        }

        if (resetStylesOnEnd)
        {
            stringBuilder.Append("\e[0m");
        }
    }

    private IEnumerable<ConsoleOutput> Normalized()
    {
        foreach (var output in outputs)
        {
            if (output is not MultiOutput multi)
            {
                yield return output;
            }
            else
            {
                foreach (var subOutput in multi.Normalized())
                {
                    yield return subOutput;
                }
            }
        }
    }
}
