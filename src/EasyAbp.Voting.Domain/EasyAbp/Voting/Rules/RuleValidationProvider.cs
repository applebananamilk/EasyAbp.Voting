using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Threading;
using Volo.Abp.Users;

namespace EasyAbp.Voting.Rules;

public abstract class RuleValidationProvider : IRuleValidationProvider, ITransientDependency
{
    public abstract string Name { get; }

    protected IAbpLazyServiceProvider LazyServiceProvider { get; }

    protected ICurrentUser CurrentUser => LazyServiceProvider.LazyGetRequiredService<ICurrentUser>();

    protected ILoggerFactory LoggerFactory => LazyServiceProvider.LazyGetRequiredService<ILoggerFactory>();

    protected ILogger Logger => LazyServiceProvider.LazyGetService<ILogger>(provider => LoggerFactory?.CreateLogger(GetType().FullName) ?? NullLogger.Instance);

    public ICancellationTokenProvider CancellationTokenProvider { get; set; }

    protected RuleValidationProvider(IServiceProvider serviceProvider)
    {
        LazyServiceProvider = serviceProvider.GetRequiredService<IAbpLazyServiceProvider>();
        CancellationTokenProvider = NullCancellationTokenProvider.Instance;
    }

    public Task ValidateAsync(RuleValidationContext context)
    {
        return ValidateAsync(context, CancellationTokenProvider.Token);
    }

    public abstract Task ValidateAsync(RuleValidationContext context, CancellationToken cancellationToken = default);
}
