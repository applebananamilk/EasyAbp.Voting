using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Localization;

namespace EasyAbp.Voting.Rules;

/// <summary>
/// 规则定义
/// </summary>
public class RuleDefinition : IHasExtraProperties
{
    /// <summary>
    /// 规则名称
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 规则显示名称
    /// </summary>
    public ILocalizableString DisplayName
    {
        get => _displayName;
        set => _displayName = Check.NotNull(value, nameof(value));
    }
    private ILocalizableString _displayName;

    /// <summary>
    /// 规则描述
    /// </summary>
    public ILocalizableString Description { get; set; }

    /// <summary>
    /// 自定义属性
    /// </summary>
    public ExtraPropertyDictionary ExtraProperties { get; protected set; }

    public RuleDefinition(
        string name,
        ILocalizableString displayName = null,
        ILocalizableString description = null)
    {
        Name = name;
        DisplayName = displayName ?? new FixedLocalizableString(name);
        Description = description;

        ExtraProperties = new ExtraPropertyDictionary();
        this.SetDefaultsForExtraProperties();
    }

    /// <summary>
    /// 添加属性
    /// </summary>
    /// <param name="propertyName">属性名</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns></returns>
    public RuleDefinition WithProperty(string propertyName, object defaultValue = null)
    {
        ExtraProperties[propertyName] = defaultValue;
        return this;
    }
}
