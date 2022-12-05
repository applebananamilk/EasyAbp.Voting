using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.DependencyInjection;

namespace EasyAbp.Voting.Rules;

public class RuleValidationProviderManager : IRuleValidationProviderManager, ISingletonDependency
{
    public List<IRuleValidationProvider> Providers => _lazyProviers.Value;

    private readonly Lazy<List<IRuleValidationProvider>> _lazyProviers;

    protected RuleOptions Options { get; }

    public RuleValidationProviderManager(
        IServiceProvider serviceProvider,
        IOptions<RuleOptions> optionsAccessor)
    {
        Options = optionsAccessor.Value;

        _lazyProviers = new Lazy<List<IRuleValidationProvider>>(
            () => Options
                .ValidationProviders
                .Select(type => serviceProvider.GetRequiredService(type) as IRuleValidationProvider)
                .ToList(),
            isThreadSafe: true
            );
    }
}
