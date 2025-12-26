using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DTOs;

public static class JsonOptions
{
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
