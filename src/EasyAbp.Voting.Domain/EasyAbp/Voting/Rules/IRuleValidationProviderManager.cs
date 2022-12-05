using System.Collections.Generic;

namespace EasyAbp.Voting.Rules;

public interface IRuleValidationProviderManager
{
    List<IRuleValidationProvider> Providers { get; }
}
