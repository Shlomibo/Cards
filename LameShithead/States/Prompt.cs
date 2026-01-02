using System;
using ConsoleUtils;

namespace LameShithead.States;

public record Prompt(string Message, Color? Color = null)
{
    public static implicit operator Prompt(string message) => new(message);
}
