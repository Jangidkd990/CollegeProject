namespace TradeBridge.Core.Interfaces;

/// <summary>
/// Interface for Power BI dashboard embedding service
/// </summary>
public interface IPowerBIService
{
    /// <summary>
    /// Gets the embed URL, token, and report ID for a Power BI report
    /// </summary>
    /// <param name="reportId">The ID of the report to embed</param>
    /// <param name="userId">The current user ID for row-level security (if applicable)</param>
    /// <returns>An object containing the embed configuration for the Power BI report</returns>
    Task<PowerBIEmbedConfig> GetEmbedConfigAsync(string reportId, string userId);
}

/// <summary>
/// Configuration for embedding a Power BI report
/// </summary>
public class PowerBIEmbedConfig
{
    public string EmbedUrl { get; set; } = string.Empty;
    public string EmbedToken { get; set; } = string.Empty;
    public string ReportId { get; set; } = string.Empty;
}