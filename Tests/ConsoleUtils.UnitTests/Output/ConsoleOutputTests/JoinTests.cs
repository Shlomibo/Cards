using System;
using AutoFixture;
using ConsoleUtils.Output;

namespace ConsoleUtils.UnitTests.Output.ConsoleOutputTests;

public class JoinTests : ConsoleOutputTestsBase
{
    [Test]
    public void WhenJoiningNothing()
    {
        ConsoleOutput joiner = "|";
        var testSubject = ConsoleOutput.Join(joiner);

        Validate(testSubject, "");
    }

    [Test]
    public void WhenJoiningOneOutput()
    {
        ConsoleOutput joiner = "|";
        ConsoleOutput value = "value";
        var testSubject = ConsoleOutput.Join(joiner, value);

        Validate(testSubject, "value");
    }

    [Test]
    public void WhenJoiningManyOutputs([Random(2, 5, 3)] int count)
    {
        ConsoleOutput joiner = "|";
        var values = Fixture.CreateMany<string>(count).ToArray();
        var testSubject = ConsoleOutput.Join(joiner, values.Select(ConsoleOutput.FromString));

        Validate(testSubject, string.Join('|', values));
    }
}
