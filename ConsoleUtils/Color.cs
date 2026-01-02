using System;

namespace ConsoleUtils;

public readonly record struct Color(ConsoleColor Foreground, ConsoleColor? Background = default)
{
    public static implicit operator Color(ConsoleColor foreground) => new(foreground);
}
