namespace TradeBridge.Web.Models;

/// <summary>
/// View model for Power BI dashboard embedding
/// </summary>
public class PowerBIDashboardViewModel
{
    public string ReportId { get; set; } = string.Empty;
    public string EmbedUrl { get; set; } = string.Empty;
    public string EmbedToken { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
}