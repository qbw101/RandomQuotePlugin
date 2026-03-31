using ClassIsland.Core.Abstractions;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Extensions.Registry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RandomQuotePlugin.Views.Components;

namespace RandomQuotePlugin;

[PluginEntrance]
public class Plugin : PluginBase
{
    public override void Initialize(HostBuilderContext context, IServiceCollection services)
    {
        // 注册随机名言组件
        // 注册后可在 【应用设置 → 组件】 中找到并拖拽到主界面
        services.AddComponent<RandomQuoteComponent>();
    }
}
