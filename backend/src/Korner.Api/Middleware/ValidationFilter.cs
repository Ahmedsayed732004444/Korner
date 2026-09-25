using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Korner.Api.Middleware;

/// <summary>
/// Runs the registered FluentValidation validator (if any) for each action argument.
/// On failure, short-circuits with 400 ProblemDetails: code "Validation.Failed",
/// errors: { field: [ "&lt;ErrorCode&gt;" ] } — each rule sets .WithErrorCode(...).
/// </summary>
public sealed class ValidationFilter(IServiceProvider serviceProvider) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null)
            {
                continue;
            }

            var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
            if (serviceProvider.GetService(validatorType) is not IValidator validator)
            {
                continue;
            }

            var validationContext = new ValidationContext<object>(argument);
            var result = await validator.ValidateAsync(validationContext, context.HttpContext.RequestAborted);

            if (!result.IsValid)
            {
                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "One or more validation errors occurred.",
                };
                problem.Extensions["code"] = "Validation.Failed";
                problem.Extensions["errors"] = result.Errors
                    .GroupBy(failure => failure.PropertyName)
                    .ToDictionary(group => group.Key, group => group.Select(failure => failure.ErrorCode).ToArray());

                context.Result = new ObjectResult(problem) { StatusCode = StatusCodes.Status400BadRequest };
                return;
            }
        }

        await next();
    }
}
