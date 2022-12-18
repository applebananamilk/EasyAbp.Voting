# Abp.Voting

[![ABP version](https://img.shields.io/badge/dynamic/xml?style=flat-square&color=yellow&label=abp&query=%2F%2FProject%2FPropertyGroup%2FAbpVersion&url=https%3A%2F%2Fraw.githubusercontent.com%2FEasyAbp%2FAbp.DataDictionary%2Fmaster%2FDirectory.Build.props)](https://abp.io)
[![NuGet](https://img.shields.io/nuget/v/EasyAbp.Voting.svg?style=flat-square)](https://www.nuget.org/packages/EasyAbp.Abp.DataDictionary.Domain.Shared)
[![NuGet Download](https://img.shields.io/nuget/dt/EasyAbp.Voting.svg?style=flat-square)](https://www.nuget.org/packages/EasyAbp.Voting.Domain.Shared)
[![GitHub stars](https://img.shields.io/github/stars/EasyAbp/EasyAbp.Voting?style=social)](https://www.github.com/EasyAbp.Voting.Domain.Shared)

ABP 投票

## 1. 介绍

**EasyAbp.Voting** 投票业务模块，基于Redis实现。你可以快速的开启一场大型投票活动。

## 2. 如何使用?

### 2.1 安装

### 2.2 定义投票规则验证提供者

通过继承`RuleValidationProvider`抽象类快速实现投票规则验证。例如，一天只有特定时间才能进行投票

```csharp
public class LimitVotingTimeRuleValidationProvider : RuleValidationProvider
{
    public const string ProviderName = "LimitVotingTime";
    public const string StartTime = nameof(StartTime);
    public const string EndTime = nameof(EndTime);

    public LimitVotingTimeRuleValidationProvider(IServiceProvider serviceProvider) : base(serviceProvider)
    {

    }

    public override string Name => ProviderName;

    public override Task ValidateAsync(RuleValidationContext context, CancellationToken cancellationToken = default)
    {
        var startTime = TimeOnly.Parse(context.GetProperty<string>(StartTime));
        var endTime = TimeOnly.Parse(context.GetProperty<string>(EndTime));

        if (startTime > endTime)
        {
            return Task.CompletedTask;
        }

        var clock = LazyServiceProvider.LazyGetRequiredService<IClock>();
        var currentTime = TimeOnly.FromDateTime(clock.Now);

        if (currentTime > startTime && currentTime < endTime)
        {
            throw new BusinessException("Voting.Rule:CurrentTimeCannotVote")
                .WithData(nameof(startTime), startTime)
                .WithData(nameof(endTime), endTime);
        }

        return Task.CompletedTask;
    }
}
```

之后，创建一个继承`RuleDefinitionProvider`的类，如下所示

```csharp
public class PreRuleDefinitionProvider : RuleDefinitionProvider
{
    public override void Define(IRuleDefinitionContext context)
    {
        context
            .AddDefinition(
                name: LimitVotingTimeRuleValidationProvider.ProviderName,
                displayName: L(LimitVotingTimeRuleValidationProvider.ProviderName))
            .WithProperty(
                propertyName: LimitVotingTimeRuleValidationProvider.StartTime,
                defaultValue: DateTime.Parse("00:00:00"))
            .WithProperty(
                propertyName: LimitVotingTimeRuleValidationProvider.EndTime,
                defaultValue: DateTime.Parse("06:00:00"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<VotingRuleResource>(name);
    }
}
```

你需要在`Define`方法中添加你的规则验证定义提供者，并添加相关名称及默认值。

参考`EasyAbp.Voting.Rule.Providers`。

## 3. 概念/术语

Todo

## 4. 路线图

Todo
