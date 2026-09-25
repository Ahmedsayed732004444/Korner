using FluentValidation;
using Korner.Api.Middleware;

namespace Korner.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKorner(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers(options => options.Filters.Add<ValidationFilter>());
        services.AddOpenApi();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        services.AddValidatorsFromAssemblyContaining<Program>();

        return services;
    }
}
