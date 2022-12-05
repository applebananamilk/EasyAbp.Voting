using Volo.Abp.Data;

namespace EasyAbp.Voting.Admin.Rules.Dtos;

public class RuleDto : IHasExtraProperties
{
    /// <summary>
    /// 规则名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 显示名称
    /// </summary>
    public string DisplayName { get; set; }

    /// <summary>
    /// 规则描述
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 该规则是否启用，默认为false。
    /// </summary>
    public virtual bool IsEnabled { get; set; }

    /// <summary>
    /// 自定义属性
    /// </summary>
    public ExtraPropertyDictionary ExtraProperties { get; set; }
}
