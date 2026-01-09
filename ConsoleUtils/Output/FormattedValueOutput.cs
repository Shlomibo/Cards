using System;
using System.Text;

namespace ConsoleUtils.Output;

internal sealed class FormattedValueOutput<T>(T? value, string? format, int? alignment) : ConsoleOutput
{
    public override void Print(StringBuilder stringBuilder)
    {
        switch ((format, alignment))
        {
            case (null, null):
                stringBuilder.Append(value);
                break;

            case (string fmt, null):
                stringBuilder.AppendFormat($$"""{0:{{fmt}}}""", value);
                break;

            case (null, int align):
                stringBuilder.AppendFormat($$"""{0,{{align}}}""", value);
                break;

            default:
                stringBuilder.AppendFormat($$"""{0,{{alignment}}:{{format}}}""", value);
                break;
        }
    }
}
