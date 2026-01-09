using System;
using AutoFixture;
using ConsoleUtils.Output;

namespace ConsoleUtils.UnitTests.Output.ConsoleOutputTests;

public class ConcatAllTests : ConsoleOutputTestsBase
{
    [Test]
    public void WhenNullOutputsAreProvided()
    {
        var subject = ConsoleOutput.ConcatAll(null);

        Validate(subject, "");
    }

    [Test]
    public void WhenNoOutputsAreProvided()
    {
        var subject = ConsoleOutput.ConcatAll();

        Validate(subject, "");
    }

    [Test]
    public void WhenOnlyOneOutputIsProvided()
    {
        var value = Fixture.Create<string>();
        var subject = ConsoleOutput.ConcatAll(value);

        Validate(subject, value);
    }

    [Test]
    public void WhenManyOutputsAreProvided()
    {
        var values = Fixture.CreateMany<string>();
        var subject = ConsoleOutput.ConcatAll(values.Select(ConsoleOutput.FromString));

        Validate(subject, string.Concat(values));
    }
}
