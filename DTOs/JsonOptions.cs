using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DTOs;

/// <summary>
/// Provides the <see cref="JsonSerializerOptions"/> and conventions to use.
/// </summary>
public static class JsonOptions
{
    /// <summary>
    /// Creates or updates a provided <paramref name="options"/> with games' conventions.
    /// </summary>
    /// <param name="options">When provided, they will be updated and returned.</param>
    /// <param name="indent"><see langword="true"/> to set json indentation.</param>
    /// <returns>
    /// If <paramref name="options"/> are provided then they are being returned after being updated.
    /// otherwise, a new instance of <see cref="JsonSerializerOptions"/>.
    /// </returns>
    public static JsonSerializerOptions SetJsonSerializationOptions(
        JsonSerializerOptions? options = null,
        bool indent = false)
    {
        options ??= new();

        options.Converters.Add(new JsonStringEnumConverter());

        options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.PropertyNameCaseInsensitive = true;
        options.WriteIndented = indent;

        return options;
    }
}
