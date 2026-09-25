using FluentAssertions;
using FluentValidation;
using Korner.Api.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Korner.UnitTests.Middleware;

public class ValidationFilterTests
{
    // Public: FluentValidation's assembly scan (AddValidatorsFromAssemblyContaining) only
    // picks up exported (public) types, so a private nested validator is silently skipped.
    public sealed record TestRequest(string Phone);

    public sealed class TestRequestValidator : AbstractValidator<TestRequest>
    {
        public TestRequestValidator()
        {
            RuleFor(request => request.Phone).NotEmpty().WithErrorCode("Test.PhoneRequired");
        }
    }

    private static IServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddValidatorsFromAssemblyContaining<TestRequestValidator>();
        return services.BuildServiceProvider();
    }

    private static (ActionExecutingContext Executing, ActionExecutionDelegate Next) CreateContext(
        IServiceProvider services,
        object argument)
    {
        var httpContext = new DefaultHttpContext { RequestServices = services };
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        var controller = new object();

        var executing = new ActionExecutingContext(
            actionContext,
            new List<IFilterMetadata>(),
            new Dictionary<string, object?> { ["request"] = argument },
            controller);

        Task<ActionExecutedContext> Next() =>
            Task.FromResult(new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), controller));

        return (executing, Next);
    }

    [Fact]
    public async Task Invalid_request_short_circuits_with_validation_failed()
    {
        var services = BuildServiceProvider();
        var filter = new ValidationFilter(services);
        var (context, next) = CreateContext(services, new TestRequest(""));

        await filter.OnActionExecutionAsync(context, next);

        var objectResult = context.Result.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

        var problem = objectResult.Value.Should().BeOfType<ProblemDetails>().Subject;
        problem.Extensions["code"].Should().Be("Validation.Failed");
        var errors = problem.Extensions["errors"].Should().BeOfType<Dictionary<string, string[]>>().Subject;
        errors["Phone"].Should().Contain("Test.PhoneRequired");
    }

    [Fact]
    public async Task Valid_request_calls_the_next_delegate()
    {
        var services = BuildServiceProvider();
        var filter = new ValidationFilter(services);
        var (context, next) = CreateContext(services, new TestRequest("01012345678"));

        await filter.OnActionExecutionAsync(context, next);

        context.Result.Should().BeNull();
    }

    [Fact]
    public async Task Argument_with_no_registered_validator_is_not_checked()
    {
        var services = BuildServiceProvider();
        var filter = new ValidationFilter(services);
        var (context, next) = CreateContext(services, "a plain string, nothing validates this");

        await filter.OnActionExecutionAsync(context, next);

        context.Result.Should().BeNull();
    }
}
