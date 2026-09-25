using FluentAssertions;
using Korner.Api.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Korner.UnitTests.Abstractions;

public class ResultExtensionsTests
{
    [Fact]
    public void ToProblem_uses_the_errors_status_code()
    {
        var error = new Error("Inventory.OutOfStock", "Requested quantity is not available", StatusCodes.Status409Conflict);
        var result = Result.Failure(error);

        var problem = (ProblemDetails)result.ToProblem().Value!;

        problem.Status.Should().Be(409);
        problem.Title.Should().Be("Requested quantity is not available");
        problem.Extensions["code"].Should().Be("Inventory.OutOfStock");
    }

    [Fact]
    public void ToProblem_defaults_to_400_when_the_error_has_no_status_code()
    {
        var error = new Error("Validation.Failed", "Invalid request", StatusCode: null);
        var result = Result.Failure(error);

        var problemResult = result.ToProblem();

        problemResult.StatusCode.Should().Be(400);
        ((ProblemDetails)problemResult.Value!).Status.Should().Be(400);
    }

    [Fact]
    public void ToProblem_throws_for_a_success_result()
    {
        var result = Result.Success();

        var act = result.ToProblem;

        act.Should().Throw<InvalidOperationException>();
    }
}
