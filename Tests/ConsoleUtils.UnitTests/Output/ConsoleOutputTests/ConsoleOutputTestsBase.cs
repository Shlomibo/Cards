using System;
using System.Text;
using AutoFixture;
using AwesomeAssertions;
using ConsoleUtils.Output;
using NUnit.Framework.Internal;

namespace ConsoleUtils.UnitTests.Output.ConsoleOutputTests;

public abstract class ConsoleOutputTestsBase
{
    protected static Fixture Fixture { get; } = new();
    protected static Randomizer Random => TestContext.CurrentContext.Random;

    protected static void Validate(ConsoleOutput testSubject, string expectedValue)
    {
        testSubject.Should().NotBeNull();
        StringBuilder sb = new(expectedValue.Length);
        testSubject.Print(sb);

        sb.ToString().Should().Be(expectedValue, "the printed string is correct");
    }

    protected static void ValidateThrows<TException>(ConsoleOutput testSubject)
        where TException : Exception
    {
        StringBuilder sb = new();

        testSubject.Invoking(output => output.Print(sb))
            .Should().Throw<TException>();
    }
}
