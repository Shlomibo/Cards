using System;
using System.Diagnostics.CodeAnalysis;
using ConsoleUtils;
using ConsoleUtils.Output;
using GamesClient.Shithead;

namespace LameShithead.States;

public abstract record State(Context Context)
{
    public async Task<State> NextState(CancellationToken cancellation)
    {
        try
        {
            return await NextStateUnsafe(cancellation);
        }
        catch (Exception ex) when (this is not ErrorState)
        {
            return new ErrorState(Context, ex);
        }
    }

    protected abstract Task<State> NextStateUnsafe(CancellationToken cancellation);

    protected Task<string> GetValueFromUser(ConsoleOutput prompt, CancellationToken cancellation)
    {
        return Task.Run(GetValueFromUserSync, cancellation);

        async Task<string> GetValueFromUserSync()
        {
            await Context.Console.WriteLine(prompt, cancellation);

            return await Context.Console.ReadLine(cancellation) ?? "";
        }
    }

    protected Task<T> GetOptionFromUser<T>(
        ConsoleOutput prompt,
        IReadOnlyCollection<(int Key, string DisplayValue, T Value)> options,
        CancellationToken cancellation)
        =>
        GetOptionFromUser(
            prompt,
            [.. options.Select(opt => (Option<T>)opt)],
            cancellation);

    protected async Task<T> GetOptionFromUser<T>(
        ConsoleOutput prompt,
        IReadOnlyCollection<Option<T>> options,
        CancellationToken cancellation)
    {
        var optionByKey = options.ToDictionary(opt => opt.Key);

        var optionsStrings = string.Join("\n\t", options
                .Select(opt => $"${opt.Key}: {opt.DisplayValue}"));

        prompt = ConsoleOutput.Interpolate($"{prompt}\n\t{options}");

        return await GetValueFromUser<T>(prompt, IsValidOption, cancellation);


        bool IsValidOption(string str, [NotNullWhen(true)] out T? value, [NotNullWhen(false)] out string? error)
        {
            value = default;
            error = default;

            if (!int.TryParse(str, out int parsed)
                || !optionByKey.TryGetValue(parsed, out var selectedOption))
            {
                error = $"'{str}' is not a valid option";
                return false;
            }
            else if (!selectedOption.Enabled)
            {
                error = $"'{parsed}' option is disabled";
                return false;
            }

            value = selectedOption.Value!;
            return true;
        }
    }

    protected async Task<T> GetValueFromUser<T>(
        ConsoleOutput prompt,
        Parser<T> parser,
        CancellationToken cancellation)
    {
        var resultStr = await GetValueFromUser(prompt, cancellation);
        T? result;

        while (!parser(resultStr, out result, out var error))
        {
            var invalidInputMessage = ConsoleOutput.Interpolate($"Invalid input: {error}")
                .Stylize(Style.Red.Forward);

            var errorPrompt = ConsoleOutput.Interpolate($"{invalidInputMessage}\n{prompt}");

            resultStr = await GetValueFromUser(errorPrompt, cancellation);
        }

        return result;
    }

    protected static Parser<string> NonEmpty(string inputName) =>
        (input, [NotNullWhen(true)] out output, [NotNullWhen(false)] out error) =>
        {
            output = null;
            error = null;

            if (input == "")
            {
                error = $"{inputName} must have a value";
                return false;
            }
            else
            {
                output = input;
                return true;
            }
        };
}

public record Context(
    IConsole Console,
    IShitheadClient ShitheadClient);

public delegate bool Parser<T>(
    string str,
    [NotNullWhen(true)] out T? result,
    [NotNullWhen(false)] out string? error);

public record Option<T>
{
    private T? _value = default;

    [MemberNotNullWhen(true, nameof(_value))]
    private bool ValueCreated { get; set; }
    private readonly Func<T> _valueFactory;

    public T Value
    {
        get
        {
            if (!ValueCreated)
            {
                _value = _valueFactory();
                ValueCreated = true;
            }

            return _value;
        }
    }

    public int Key { get; }
    public string DisplayValue { get; }
    public bool Enabled { get; }

    public Option(int key, string displayValue, T value, bool enabled = true)
        : this(key, displayValue, () => value, enabled)
    {
        Key = key;
        DisplayValue = displayValue;
        Enabled = enabled;
        _value = value;
        ValueCreated = true;
    }

    public Option(int key, string displayValue, Func<T> valueFactory, bool enabled = true)
    {
        Key = key;
        DisplayValue = displayValue;
        _valueFactory = valueFactory;
        Enabled = enabled;
    }

    public static implicit operator Option<T>((int Key, string DisplayValue, T Value) triple) =>
        new(triple.Key, triple.DisplayValue, triple.Value);
}
