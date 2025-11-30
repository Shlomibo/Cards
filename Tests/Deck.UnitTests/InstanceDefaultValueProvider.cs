using System;

using Moq;

namespace Deck.UnitTests;

public static class InstanceDefaultValueProvider
{
    public static InstanceDefaultValueProvider<T> Create<T>(T instance)
        where T : class
        =>
        new(instance);
}

public sealed class InstanceDefaultValueProvider<T> : LookupOrFallbackDefaultValueProvider
where T : class
{
    public InstanceDefaultValueProvider(T instance)
    {
        Register(typeof(T), (_, _) => instance);
    }
}
