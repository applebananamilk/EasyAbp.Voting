# Abp.Voting

[![ABP version](https://img.shields.io/badge/dynamic/xml?style=flat-square&color=yellow&label=abp&query=//Project/PropertyGroup/AbpVersion&url=https://raw.githubusercontent.com/applebananamilk/EasyAbp.Voting/master/Directory.Build.props)](https://abp.io)
[![NuGet](https://img.shields.io/nuget/v/EasyAbp.Voting.Domain.Shared.svg?style=flat-square)](https://www.nuget.org/packages/EasyAbp.Voting.Domain.Shared)
[![NuGet Download](https://img.shields.io/nuget/dt/EasyAbp.Voting.Domain.Shared.svg?style=flat-square)](https://www.nuget.org/packages/EasyAbp.Voting.Domain.Shared)
[![GitHub stars](https://img.shields.io/github/stars/applebananamilk/EasyAbp.Voting?style=social)](https://github.com/applebananamilk/EasyAbp.Voting)

ABP 投票活动

## 1. 介绍

**EasyAbp.Voting** 投票业务模块，基于 Redis 实现。你可以快速的开启一场大型投票活动。

## 2. 如何使用?

### 2.1 安装

1. 安装以下 NuGet 包 ([文档](https://github.com/EasyAbp/EasyAbpGuide/blob/master/docs/How-To.md#add-nuget-packages))

    - EasyAbp.Voting.Application
    - EasyAbp.Voting.Admin.Application
    - EasyAbp.Voting.Application.Contracts
    - EasyAbp.Voting.Admin.Application.Contracts
    - EasyAbp.Voting.Domain
    - EasyAbp.Voting.Domain.Shared
    - EasyAbp.Voting.EntityFrameworkCore
    - EasyAbp.Voting.HttpApi
    - EasyAbp.Voting.Admin.HttpApi
    - (可选、预定义的一些规则) EasyAbp.Voting.Rule.Providers

1. 添加 `DependsOn(typeof(VotingXxxModule))` 模块依赖。([文档](https://github.com/EasyAbp/EasyAbpGuide/blob/master/docs/How-To.md#add-module-dependencies))

1. 在**MyProjectMigrationsDbContext.cs**的`OnModelCreating()` 方法中添加 `builder.ConfigureVoting();`

1. 添加 EF Core 迁移并更新数据库 [ABP 文档](https://docs.abp.io/en/abp/latest/Tutorials/Part-1?UI=MVC&DB=EF#add-database-migration)

1. 此模块**需要配置 Redis**，请参阅 [ABP 文档](https://docs.abp.io/zh-Hans/abp/latest/Redis-Cache)

### 2.2 自定义投票规则验证提供者

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
