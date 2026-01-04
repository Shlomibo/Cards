using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace ConsoleUtils.Output;

[InterpolatedStringHandler]
public sealed class InterpolatedConsoleOutput : IConsoleOutput
{
    private readonly string[] _strings;
    private readonly (object? value, int? alignment, string? format)[] _values;
    private int _index;

    public InterpolatedConsoleOutput(int literalLength, int formattedCount)
    {
        _strings = new string[formattedCount + 1];
        _values = new (object? value, int? alignment, string? format)[formattedCount];
    }

    public void Print(StringBuilder stringBuilder)
    {
        for (int i = 0; i < _values.Length; i++)
        {
            stringBuilder.Append(_strings[i]);

            var (value, alignment, format) = _values[i];

            if (value is IConsoleOutput output)
            {
                if (!alignment.HasValue)
                {
                    output.Print(stringBuilder);
                }
                else
                {
                    StringBuilder unaligned = new();
                    output.Print(unaligned);
                    stringBuilder.AppendFormat($$"""{0,{{alignment}}}""", unaligned);
                }
            }
            else
            {
                switch ((alignment, format))
                {
                    case (null, null):
                        stringBuilder.Append(value);
                        break;
                    case (int align, null):
                        stringBuilder.AppendFormat($$"""{0,{{align}}}""", value);
                        break;
                    case (null, string fmt):
                        stringBuilder.AppendFormat($$"""{0:{{fmt}}}""");
                        break;
                    default:
                        stringBuilder.AppendFormat($$"""{0,{{alignment}}:{{format}}}""");
                        break;
                }
            }
        }

        stringBuilder.Append(_strings[^1]);
    }

    public void AppendLiteral(string s)
    {
        _strings[_index] = s;
    }

    public void AppendFormatted<T>(T t)
    {
        _values[_index++] = (t, null, null);
    }

    public void AppendFormatted<T>(T t, int alignment)
    {
        _values[_index++] = (t, alignment, null);
    }

    public void AppendFormatted<T>(T t, int alignment, string? format)
    {
        _values[_index++] = (t, alignment, format);
    }

    public void AppendFormatted<T>(T t, string? format)
    {
        _values[_index++] = (t, null, format);
    }
}
