using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Volo.Abp.DependencyInjection;

namespace EasyAbp.Voting.Rules;

public class RuleDefinitionManager : IRuleDefinitionManager, ISingletonDependency
{
    protected RuleOptions Options { get; }

    protected IDictionary<string, RuleDefinition> RuleDefinitions => _lazyRuleDefinitions.Value;
    private readonly Lazy<Dictionary<string, RuleDefinition>> _lazyRuleDefinitions;

    private readonly IServiceScopeFactory _serviceScopeFactory;

    public RuleDefinitionManager(
        IOptions<RuleOptions> optionsAccessor,
        IServiceScopeFactory serviceScopeFactory)
    {
        Options = optionsAccessor.Value;
        _serviceScopeFactory = serviceScopeFactory;

        _lazyRuleDefinitions = new Lazy<Dictionary<string, RuleDefinition>>(
            CreateRuleDefinitions,
            isThreadSafe: true
            );
    }

    public IReadOnlyList<RuleDefinition> GetAll()
    {
        return RuleDefinitions.Values.ToImmutableList();
    }

    public RuleDefinition Get(string name)
    {
        return RuleDefinitions.GetOrDefault(name);
    }

    protected virtual Dictionary<string, RuleDefinition> CreateRuleDefinitions()
    {
        var context = new RuleDefinitionContext();
        var rules = new Dictionary<string, RuleDefinition>();

        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var providers = Options
                .DefinitionProviders
                .Select(p => scope.ServiceProvider.GetRequiredService(p) as IRuleDefinitionProvider)
                .ToList();

            foreach (var provider in providers)
            {
                provider.Define(context);
            }
        }

        return context.RuleDefinitions.ToDictionary(rule => rule.Name, rule => rule);
    }
}
