using System.Net.Http;
using System.Text.Json;
using Avalonia;
using Avalonia.Threading;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using RandomQuotePlugin.Models;

namespace RandomQuotePlugin.Views.Components;

[ComponentInfo(
    "A3F2C8B1-4D6E-47A9-8C2F-1E9D5B3A7F04",
    "随机名言",
    "\uE9B0",
    "每隔 10 秒获取并展示一条随机名言"
)]
public partial class RandomQuoteComponent : ComponentBase
{
    private static readonly HttpClient s_http = new()
    {
        Timeout = TimeSpan.FromSeconds(8)
    };

    private const string ApiUrl =
        "https://backend.appmiaoda.com/projects/supabase291883360291172352/functions/v1/get-random-quote?max_length=45";

    // 正文属性
    public static readonly StyledProperty<string> ContentProperty =
        AvaloniaProperty.Register<RandomQuoteComponent, string>(
            nameof(content),
            defaultValue: "正在加载名言…");

    public string content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    // 来源属性
    public static readonly StyledProperty<string> SourceProperty =
        AvaloniaProperty.Register<RandomQuoteComponent, string>(
            nameof(source),
            defaultValue: string.Empty);

    public string source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    private readonly DispatcherTimer _timer;

    public RandomQuoteComponent()
    {
        InitializeComponent();

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(10)
        };
        _timer.Tick += async (_, _) => await FetchQuoteAsync();

        AttachedToVisualTree += OnAttachedToVisualTree;
        DetachedFromVisualTree += OnDetachedFromVisualTree;
    }

    private async void OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        await FetchQuoteAsync();
        _timer.Start();
    }

    private void OnDetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        _timer.Stop();
    }

    // 🔥 核心修复：正确解析嵌套JSON
    private async Task FetchQuoteAsync()
    {
        try
        {
            var json = await s_http.GetStringAsync(ApiUrl);
            // 反序列化为完整响应对象
            var response = JsonSerializer.Deserialize<QuoteApiResponse>(json);

            // 校验响应
            if (response == null || !response.success || response.data == null)
            {
                await SetErrorAsync("获取名言失败");
                return;
            }

            var quote = response.data; // 取出data中的名言数据

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                content = string.IsNullOrWhiteSpace(quote.content) ? "（无内容）" : quote.content.Trim();
                source = quote.source?.Trim() ?? string.Empty;
            });
        }
        catch (HttpRequestException httpEx)
        {
            await SetErrorAsync($"网络错误：{httpEx.StatusCode}");
        }
        catch (TaskCanceledException)
        {
            await SetErrorAsync("请求超时");
        }
        catch (JsonException)
        {
            await SetErrorAsync("解析响应失败");
        }
        catch (Exception ex)
        {
            await SetErrorAsync($"未知错误：{ex.Message}");
        }
    }

    private async Task SetErrorAsync(string message)
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            content = $"⚠ {message}";
            source = string.Empty;
        });
    }
}
