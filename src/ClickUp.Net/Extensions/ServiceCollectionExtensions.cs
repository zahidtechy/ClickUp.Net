using ClickUp.Net.Authentication.OAuth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ClickUpHttp = ClickUp.Net.Infrastructure.ClickUpHttpClient;

namespace ClickUp.Net;

/// <summary>
/// Registers the ClickUp client with dependency injection and <see cref="IHttpClientFactory"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the ClickUp client, OAuth helper, and a named HTTP client.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">A callback that sets <see cref="ClickUpOptions"/>.</param>
    /// <returns>The HTTP client builder, so callers can attach handlers or policies.</returns>
    public static IHttpClientBuilder AddClickUp(this IServiceCollection services, Action<ClickUpOptions> configure)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configure is null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        services.AddOptions<ClickUpOptions>().Configure(configure).PostConfigure(Normalize);
        return Register(services);
    }

    /// <summary>
    /// Adds the ClickUp client using a configuration section such as <c>ClickUp</c>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration section to bind to <see cref="ClickUpOptions"/>.</param>
    /// <returns>The HTTP client builder, so callers can attach handlers or policies.</returns>
    public static IHttpClientBuilder AddClickUp(this IServiceCollection services, IConfiguration configuration)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configuration is null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        services.AddOptions<ClickUpOptions>().Bind(configuration).PostConfigure(Normalize);
        return Register(services);
    }

    private static IHttpClientBuilder Register(IServiceCollection services)
    {
        services.TryAddSingleton(serviceProvider => new ClickUpClientFactory(
            serviceProvider.GetRequiredService<IHttpClientFactory>(),
            serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<ClickUpOptions>>(),
            serviceProvider.GetService<ILoggerFactory>() ?? NullLoggerFactory.Instance));
        services.TryAddSingleton<IClickUpClientFactory>(serviceProvider => serviceProvider.GetRequiredService<ClickUpClientFactory>());
        services.TryAddSingleton<IClickUpOAuthClient>(serviceProvider => new ClickUpOAuthClient(
            serviceProvider.GetRequiredService<IHttpClientFactory>(),
            serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<ClickUpOptions>>(),
            serviceProvider.GetService<ILoggerFactory>() ?? NullLoggerFactory.Instance));
        services.TryAddTransient<IClickUpClient>(serviceProvider => serviceProvider.GetRequiredService<ClickUpClientFactory>().CreateDefault());

        return services.AddHttpClient(ClickUpHttp.HttpClientName, (serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<ClickUpOptions>>().Value;
            client.BaseAddress = ClickUpOptions.CreateBaseUri(options.BaseUrl);
            client.Timeout = options.Timeout;
            client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
            client.DefaultRequestHeaders.UserAgent.ParseAdd("ClickUp.Net/1.0.0");
        });
    }

    private static void Normalize(ClickUpOptions options)
    {
        options.BaseUrl = ClickUpOptions.CreateBaseUri(options.BaseUrl).AbsoluteUri;
        if (options.Timeout <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(options.Timeout), "ClickUp timeout must be positive.");
        }

        if (options.Retry is null)
        {
            options.Retry = new ClickUpRetryOptions();
        }

        if (options.Retry.MaxRetries < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(options.Retry.MaxRetries), "ClickUp retry count cannot be negative.");
        }

        if (options.Retry.MaxDelay < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(options.Retry.MaxDelay), "ClickUp retry delay cannot be negative.");
        }
    }
}
