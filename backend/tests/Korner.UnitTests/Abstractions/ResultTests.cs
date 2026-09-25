using FluentAssertions;
using Korner.Api.Abstractions;

namespace Korner.UnitTests.Abstractions;

public class ResultTests
{
    private static readonly Error SampleError = new("Sample.Error", "Something went wrong", 409);

    [Fact]
    public void Success_has_no_error()
    {
        var result = Result.Success();

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void Failure_carries_the_given_error()
    {
        var result = Result.Failure(SampleError);

        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(SampleError);
    }

    [Fact]
    public void Generic_success_carries_the_value()
    {
        var result = Result.Success(42);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
    }

    [Fact]
    public void Generic_failure_has_no_value()
    {
        var result = Result.Failure<int>(SampleError);

        result.IsFailure.Should().BeTrue();
        var act = () => result.Value;
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Failure_with_no_error_is_not_a_valid_state()
    {
        var act = () => Result.Failure<int>(Error.None);

        act.Should().Throw<InvalidOperationException>();
    }
}
