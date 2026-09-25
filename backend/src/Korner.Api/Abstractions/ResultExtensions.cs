using Microsoft.AspNetCore.Mvc;

namespace Korner.Api.Abstractions;

public static class ResultExtensions
{
    public static ObjectResult ToProblem(this Result result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException("Cannot convert success to problem.");
        }

        var status = result.Error.StatusCode ?? StatusCodes.Status400BadRequest;
        var problem = new ProblemDetails { Status = status, Title = result.Error.Description };
        problem.Extensions["code"] = result.Error.Code;
        return new ObjectResult(problem) { StatusCode = status };
    }
}
