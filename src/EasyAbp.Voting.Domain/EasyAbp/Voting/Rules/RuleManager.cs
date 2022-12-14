using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace EasyAbp.Voting.Rules;

public class RuleManager : DomainService
{
    protected IRuleRepository RuleRepository { get; }
    protected IRuleDefinitionManager RuleDefinitionManager { get; }
    protected IStringLocalizerFactory StringLocalizerFactory { get; }

    public RuleManager(
        IRuleRepository ruleRepository,
        IRuleDefinitionManager ruleDefinitionManager,
        IStringLocalizerFactory stringLocalizerFactory)
    {
        RuleRepository = ruleRepository;
        RuleDefinitionManager = ruleDefinitionManager;
        StringLocalizerFactory = stringLocalizerFactory;
    }

    /// <summary>
    /// 获取活动规则列表。
    /// </summary>
    /// <param name="activityId">活动Id</param>
    /// <remarks>
    /// 获取所有预定义的规则，默认全部禁用。
    /// </remarks>
    /// <returns></returns>
    public virtual async Task<List<RuleDefinitionWithOnOff>> GetRuleListAsync(Guid activityId)
    {
        var ruleDefinitions = RuleDefinitionManager.GetAll();

        var rulePOs = await RuleRepository.GetListAsync(p => p.ActivityId == activityId);

        var rules = new List<RuleDefinitionWithOnOff>(ruleDefinitions.Count);

        foreach (var ruleDefinition in ruleDefinitions)
        {
            var rulePO = rulePOs.Find(p => p.Name == ruleDefinition.Name);

            var rule = new RuleDefinitionWithOnOff(
                ruleDefinition.Name,
                ruleDefinition.DisplayName?.Localize(StringLocalizerFactory),
                ruleDefinition.Description?.Localize(StringLocalizerFactory));
            rule.SetExtraProperties(ruleDefinition.ExtraProperties);

            if (rulePO != null)
            {
                rule.IsEnabled = rulePO.IsEnabled;
                // 使用规则定义的属性，防止出现不需要的Key
                foreach (var property in ruleDefinition.ExtraProperties)
                {
                    rule.ExtraProperties[property.Key] = rulePO.ExtraProperties[property.Key];
                }
            }

            rules.Add(rule);
        }

        return rules;
    }

    /// <summary>
    /// 修改活动规则。
    /// </summary>
    /// <param name="activityId">活动Id</param>
    /// <param name="rules">规则列表</param>
    /// <returns></returns>
    /// <exception cref="BusinessException"></exception>
    public async Task UpdateRuleListAsync(Guid activityId, List<RuleDefinitionWithOnOff> rules)
    {
        var ruleDefinitions = RuleDefinitionManager.GetAll();
        var rulePOs = await RuleRepository.GetListAsync(p => p.ActivityId == activityId);

        foreach (var rule in rules)
        {
            var rulePO = rulePOs.Find(p => p.Name == rule.Name);

            var ruleDefinition = ruleDefinitions.FirstOrDefault(p => p.Name == rule.Name);

            if (ruleDefinition == null)
            {
                throw new BusinessException(VotingErrorCodes.RuleNameNotFound)
                    .WithData(nameof(rule.Name), rule.Name);
            }

            if (rulePO == null)
            {
                rulePO = new Rule(
                    GuidGenerator.Create(),
                    activityId,
                    rule.Name,
                    rule.IsEnabled
                    );
                SetExtraProperties(rulePO, ruleDefinition, rule);
                await RuleRepository.InsertAsync(rulePO, true);
            }
            else
            {
                rulePO.IsEnabled = rule.IsEnabled;
                SetExtraProperties(rulePO, ruleDefinition, rule);
                await RuleRepository.UpdateAsync(rulePO, true);
            }
        }
    }

    /// <summary>
    /// 设置额外属性
    /// </summary>
    /// <param name="rulePO">数据库存储</param>
    /// <param name="ruleDefinition">规则定义</param>
    /// <param name="rule">规则定义包含开关</param>
    private void SetExtraProperties(Rule rulePO, RuleDefinition ruleDefinition, RuleDefinitionWithOnOff rule)
    {
        foreach (var property in ruleDefinition.ExtraProperties)
        {
            rulePO.ExtraProperties[property.Key] = rule.ExtraProperties[property.Key];
        }
    }
}
