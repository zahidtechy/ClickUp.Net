using ClickUp.Net.Authentication.OAuth;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ClickUp.Net.Tests;

internal sealed class ClickUpClientFixture : IDisposable
{
    private readonly ServiceProvider _provider;

    public ClickUpClientFixture(RecordingHandler handler, Action<ClickUpOptions>? configure = null, bool logging = false)
    {
        Handler = handler;
        var services = new ServiceCollection();
        if (logging)
        {
            Logger = new ListLoggerProvider();
            services.AddLogging(builder =>
            {
                builder.AddProvider(Logger);
                builder.SetMinimumLevel(LogLevel.Debug);
            });
        }

        services.AddClickUp(options =>
        {
            options.PersonalToken = "pk_unit_test_token";
            options.Retry.Enabled = false;
            configure?.Invoke(options);
        }).ConfigurePrimaryHttpMessageHandler(() => Handler);

        _provider = services.BuildServiceProvider();
        Client = _provider.GetRequiredService<IClickUpClient>();
        OAuth = _provider.GetRequiredService<IClickUpOAuthClient>();
        Factory = _provider.GetRequiredService<IClickUpClientFactory>();
    }

    public RecordingHandler Handler { get; }

    public IClickUpClient Client { get; }

    public IClickUpOAuthClient OAuth { get; }

    public IClickUpClientFactory Factory { get; }

    public ListLoggerProvider? Logger { get; }

    public void Dispose()
    {
        _provider.Dispose();
    }
}

internal sealed class ListLoggerProvider : ILoggerProvider
{
    public List<string> Messages { get; } = new();

    public ILogger CreateLogger(string categoryName)
    {
        return new ListLogger(Messages);
    }

    public void Dispose()
    {
    }

    private sealed class ListLogger : ILogger
    {
        private readonly List<string> _messages;

        public ListLogger(List<string> messages)
        {
            _messages = messages;
        }

        public IDisposable BeginScope<TState>(TState state) => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            _messages.Add(formatter(state, exception));
        }
    }

    private sealed class NullScope : IDisposable
    {
        public static readonly NullScope Instance = new();

        public void Dispose()
        {
        }
    }
}
