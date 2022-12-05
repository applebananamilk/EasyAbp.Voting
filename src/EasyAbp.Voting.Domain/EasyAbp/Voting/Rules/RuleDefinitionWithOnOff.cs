using Volo.Abp.Data;

namespace EasyAbp.Voting.Rules;

/// <summary>
/// 规则定义包含开关
/// </summary>
public class RuleDefinitionWithOnOff : IHasExtraProperties
{
    /// <summary>
    /// 规则名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 规则显示名称
    /// </summary>
    public string DisplayName { get; set; }

    /// <summary>
    /// 规则描述
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 自定义属性
    /// </summary>
    public ExtraPropertyDictionary ExtraProperties { get; set; }

    /// <summary>
    /// 该规则是否启用，默认为false。
    /// </summary>
    public virtual bool IsEnabled { get; set; }

    public RuleDefinitionWithOnOff()
    {
        ExtraProperties = new ExtraPropertyDictionary();
        this.SetDefaultsForExtraProperties();
    }

    public RuleDefinitionWithOnOff(
        string name,
        string displayName = null,
        string description = null,
        bool isEnabled = false) : this()
    {
        Name = name;
        DisplayName = displayName;
        Description = description;
        IsEnabled = isEnabled;
    }

    public void SetExtraProperties(ExtraPropertyDictionary externalExtraProperties)
    {
        ExtraProperties.Clear();
        foreach (var property in externalExtraProperties)
        {
            ExtraProperties[property.Key] = externalExtraProperties[property.Key];
        }
    }
}
