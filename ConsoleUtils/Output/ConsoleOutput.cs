using System;

namespace ConsoleUtils.Output;

public static class ConsoleOutput
{
    public static IConsoleOutput FromString(string? output) =>
        new StringConsoleOutput(output);

    public static IConsoleOutput Stylize(this IConsoleOutput output, HashSet<Style> styles) =>
        new OutputStyle(output, styles);

    public static IConsoleOutput Stylize(this IConsoleOutput output, params IEnumerable<Style> styles) =>
        new OutputStyle(output, [.. styles]);

    public static IConsoleOutput Concat(this IConsoleOutput output, params IEnumerable<IConsoleOutput> outputs) =>
        output.Concat(resetStylesOnEnd: false, outputs);

    public static IConsoleOutput Concat(this IConsoleOutput output, bool resetStylesOnEnd, params IEnumerable<IConsoleOutput> outputs) =>
        new MultiOutput([output, .. outputs], resetStylesOnEnd);

    public static IConsoleOutput Interpolate(InterpolatedConsoleOutput interpolatedOutput) =>
        interpolatedOutput;
}


