using System;
using AutoFixture;
using ConsoleUtils.Output;

namespace ConsoleUtils.UnitTests.Output.ConsoleOutputTests;

public class ConcatTests : ConsoleOutputTestsBase
{
    private const string RESET = "\e[0m";

    [Test]
    public void WhenConcatenatingNothing()
    {
        var value = Fixture.Create<string>();
        ConsoleOutput output = value;
        var testSubject = output.Concat();

        Validate(testSubject, value);
    }

    [Test]
    public void WhenConcatenatingAnotherOutput()
    {
        var value = Fixture.Create<string>();
        var another = Fixture.Create<string>();
        ConsoleOutput output = value;
        var testSubject = output.Concat(another);

        Validate(testSubject, value + another);
    }

    [Test]
    public void WhenConcatenatingManyOutputs()
    {
        var value = Fixture.Create<string>();
        var others = Fixture.CreateMany<string>();
        ConsoleOutput output = value;
        var testSubject = output.Concat(others.Select(ConsoleOutput.FromString));

        Validate(testSubject, string.Concat([value, .. others]));
    }

    [Test]
    public void WhenConcatenatingNothingAndExplicitlyNotResettingStyles()
    {
        var value = Fixture.Create<string>();
        ConsoleOutput output = value;
        var testSubject = output.Concat(resetStylesOnEnd: false);

        Validate(testSubject, value);
    }

    [Test]
    public void WhenConcatenatingAnotherOutputAndExplicitlyNotResettingStyles()
    {
        var value = Fixture.Create<string>();
        var another = Fixture.Create<string>();
        ConsoleOutput output = value;
        var testSubject = output.Concat(resetStylesOnEnd: false, another);

        Validate(testSubject, value + another);
    }

    [Test]
    public void WhenConcatenatingManyOutputsAndExplicitlyNotResettingStyles()
    {
        var value = Fixture.Create<string>();
        var others = Fixture.CreateMany<string>();
        ConsoleOutput output = value;
        var testSubject = output.Concat(
            resetStylesOnEnd: false,
            others.Select(ConsoleOutput.FromString));

        Validate(testSubject, string.Concat([value, .. others]));
    }

    [Test]
    public void WhenConcatenatingNothingAndResettingStyles()
    {
        var value = Fixture.Create<string>();
        ConsoleOutput output = value;
        var testSubject = output.Concat(resetStylesOnEnd: true);

        Validate(testSubject, value + RESET);
    }

    [Test]
    public void WhenConcatenatingAnotherOutputAndResettingStyles()
    {
        var value = Fixture.Create<string>();
        var another = Fixture.Create<string>();
        ConsoleOutput output = value;
        var testSubject = output.Concat(resetStylesOnEnd: true, another);

        Validate(testSubject, value + another + RESET);
    }

    [Test]
    public void WhenConcatenatingManyOutputsAndResettingStyles()
    {
        var value = Fixture.Create<string>();
        var others = Fixture.CreateMany<string>();
        ConsoleOutput output = value;
        var testSubject = output.Concat(
            resetStylesOnEnd: true,
            others.Select(ConsoleOutput.FromString));

        Validate(testSubject, string.Concat([value, .. others, RESET]));
    }
}
