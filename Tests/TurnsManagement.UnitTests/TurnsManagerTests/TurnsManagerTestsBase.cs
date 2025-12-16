using NUnit.Framework.Internal;

namespace TurnsManagement.UnitTests.TurnsManagerTests;

public abstract class TurnsManagerTestsBase
{
    protected const int MinRandomPLayersCount = 4;
    protected const int MaxRandomPLayersCount = 6;
    protected static Randomizer Random => TestContext.CurrentContext.Random;

    protected static TurnsManager GetTestSubject(
        int? playersCount = null,
        TurnsDirection? direction = null,
        Func<int, int>? current = null)
    {
        playersCount ??= Random.Next(
            MinRandomPLayersCount,
            MaxRandomPLayersCount);

        return new(
            playersCount.Value,
            direction)
        {
            Current = current?.Invoke(playersCount.Value) ?? Random.Next(playersCount.Value),
        };
    }
}
