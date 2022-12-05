using EasyAbp.Voting.Rules;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Data;
using Volo.Abp.Validation;

namespace EasyAbp.Voting.Admin.Rules.Dtos;

public class RuleUpdateInput : IHasExtraProperties
{
    /// <summary>
    /// 规则唯一名称。
    /// </summary>
    [Required]
    [DynamicStringLength(typeof(RuleConsts), nameof(RuleConsts.MaxNameLength))]
    public string Name { get; set; }

    /// <summary>
    /// 该规则是否启用，默认为false。
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// 规则属性
    /// </summary>
    [Required]
    public ExtraPropertyDictionary ExtraProperties { get; set; }
}
