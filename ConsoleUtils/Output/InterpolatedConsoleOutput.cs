using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace ConsoleUtils.Output;

/// <summary>
/// Represents console output created via interpolated strings.
/// </summary>
[InterpolatedStringHandler]
public sealed class InterpolatedConsoleOutput : ConsoleOutput
{
    private readonly string[] _strings;
    private readonly (object? value, int? alignment, string? format)[] _values;
    private int _index;

    /// <summary>
    /// Initializes a new instance of the <see cref="InterpolatedConsoleOutput"/> class.
    /// </summary>
    /// <param name="literalLength">The length of the literal parts of the interpolated string.</param>
    /// <param name="formattedCount">The number of formatted parts of the interpolated string.</param>
    public InterpolatedConsoleOutput(int literalLength, int formattedCount)
    {
        _strings = new string[formattedCount + 1];
        _values = new (object? value, int? alignment, string? format)[formattedCount];
    }

    /// <inheritdoc/>
    public override void Print(StringBuilder stringBuilder)
    {
        for (int i = 0; i < _values.Length; i++)
        {
            stringBuilder.Append(_strings[i]);

            var (value, alignment, format) = _values[i];

            if (value is not ConsoleOutput output)
            {
                output = new FormattedValueOutput<object>(value, format, alignment);
                alignment = null;
            }

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

        stringBuilder.Append(_strings[^1]);
    }

    /// <summary>
    /// Appends a literal string to the interpolated console output.
    /// </summary>
    /// <param name="s">The literal string to append.</param>
    public void AppendLiteral(string s)
    {
        _strings[_index] = s;
    }

    /// <summary>
    /// Appends a formatted value to the interpolated console output.
    /// </summary>
    /// <typeparam name="T">The type of the value to append.</typeparam>
    /// <param name="t">The value to append.</param>
    public void AppendFormatted<T>(T t)
    {
        _values[_index++] = (t, null, null);
    }

    /// <summary>
    /// Appends a formatted value with alignment to the interpolated console output.
    /// </summary>
    /// <typeparam name="T">The type of the value to append.</typeparam>
    /// <param name="t">The value to append.</param>
    /// <param name="alignment">The alignment for the value.</param>
    public void AppendFormatted<T>(T t, int alignment)
    {
        _values[_index++] = (t, alignment, null);
    }

    /// <summary>
    /// Appends a formatted value with format string to the interpolated console output.
    /// </summary>
    /// <typeparam name="T">The type of the value to append.</typeparam>
    /// <param name="t">The value to append.</param>
    /// <param name="format">The format string for the value.</param>
    public void AppendFormatted<T>(T t, string? format)
    {
        _values[_index++] = (t, null, format);
    }

    /// <summary>
    /// Appends a formatted value with alignment and format string to the interpolated console output.
    /// </summary>
    /// <typeparam name="T">The type of the value to append.</typeparam>
    /// <param name="t">The value to append.</param>
    /// <param name="alignment">The alignment for the value.</param>
    /// <param name="format">The format string for the value.</param>
    public void AppendFormatted<T>(T t, int alignment, string? format)
    {
        _values[_index++] = (t, alignment, format);
    }
}
