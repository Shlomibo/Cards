using System;
using System.Diagnostics.CodeAnalysis;

namespace DTOs.Responses;

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
