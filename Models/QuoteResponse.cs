namespace RandomQuotePlugin.Models;

/// <summary>
/// 接口完整响应体
/// </summary>
public class QuoteApiResponse
{
    public bool success { get; set; }
    public int code { get; set; }
    public string message { get; set; } = string.Empty;
    public QuoteItem? data { get; set; }
}

/// <summary>
/// 名言数据实体
/// </summary>
public class QuoteItem
{
    public string id { get; set; } = string.Empty;
    public string content { get; set; } = string.Empty;
    public string source { get; set; } = string.Empty;
    public DateTime created_at { get; set; }
    public Submitter? submitter { get; set; }
}

/// <summary>
/// 提交者信息
/// </summary>
public class Submitter
{
    public string username { get; set; } = string.Empty;
}