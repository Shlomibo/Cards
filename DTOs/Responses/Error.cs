using System;
using System.Diagnostics.CodeAnalysis;

namespace DTOs.Responses;

/// <summary>
/// An error response.
/// </summary>
public record Error
{
    /// <summary>
    /// The error message.
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    /// Creates a new error response.
    /// </summary>
    public Error()
    {
    }

    /// <summary>
    /// Creates a new error response with the provided message.
    /// </summary>
    /// <param name="message">The error message.</param>
    [SetsRequiredMembers]
    public Error(string message)
    {
        Message = message;
    }
}
