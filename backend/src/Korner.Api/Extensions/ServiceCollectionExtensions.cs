using FluentValidation;
using Korner.Api.Configurations;
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
        services.AddOutputCache();
        services.AddSingleton(TimeProvider.System);

        services
            .AddValidatedOptions<JwtOptions>(configuration, JwtOptions.SectionName)
            .AddValidatedOptions<AuthOptions>(configuration, AuthOptions.SectionName)
            .AddValidatedOptions<GoogleAuthOptions>(configuration, GoogleAuthOptions.SectionName)
            .AddValidatedOptions<MailSettingsOptions>(configuration, MailSettingsOptions.SectionName)
            .AddValidatedOptions<PaymobSettingsOptions>(configuration, PaymobSettingsOptions.SectionName)
            .AddValidatedOptions<HangfireSettingsOptions>(configuration, HangfireSettingsOptions.SectionName)
            .AddValidatedOptions<StorageOptions>(configuration, StorageOptions.SectionName);

        return services;
    }

    /// <summary>Binds, validates (DataAnnotations) and re-validates on every start — the app refuses
    /// to start with a missing or invalid required option (technical design §2 rule 6).</summary>
    private static IServiceCollection AddValidatedOptions<TOptions>(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName)
        where TOptions : class
    {
        services
            .AddOptions<TOptions>()
            .Bind(configuration.GetSection(sectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}
