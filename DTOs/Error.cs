using System;
using System.Diagnostics.CodeAnalysis;

namespace DTOs;

public record Error
{
    public required string Message { get; init; }

    public Error()
    {
    }

    [SetsRequiredMembers]
    public Error(string message)
    {
        Message = message;
    }
}
