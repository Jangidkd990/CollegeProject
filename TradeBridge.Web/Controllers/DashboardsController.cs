namespace TradeBridge.Web.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TradeBridge.Core.Interfaces;
using TradeBridge.Web.Models;
using System.Threading.Tasks;

[Authorize]
public class DashboardsController : Controller
{
    private readonly IPowerBIService _powerBIService;
    private readonly IConfiguration _configuration;

    public DashboardsController(IPowerBIService powerBIService, IConfiguration configuration)
    {
        _powerBIService = powerBIService;
        _configuration = configuration;
    }

    // Sales dashboard
    public async Task<IActionResult> Sales()
    {
        var reportId = _configuration["PowerBI:ReportId"];
        var userId = User.Identity?.Name ?? "anonymous";

        var embedConfig = await _powerBIService.GetEmbedConfigAsync(reportId!, userId);

        var viewModel = new PowerBIDashboardViewModel
        {
            ReportId = embedConfig.ReportId,
            EmbedUrl = embedConfig.EmbedUrl,
            EmbedToken = embedConfig.EmbedToken,
            Title = "Sales Analytics Dashboard"
        };

        return View(viewModel);
    }

    // Procurement dashboard
    public async Task<IActionResult> Procurement()
    {
        var reportId = _configuration["PowerBI:ReportId"]; // In real app, use separate report IDs
        var userId = User.Identity?.Name ?? "anonymous";

        var embedConfig = await _powerBIService.GetEmbedConfigAsync(reportId!, userId);

        var viewModel = new PowerBIDashboardViewModel
        {
            ReportId = embedConfig.ReportId,
            EmbedUrl = embedConfig.EmbedUrl,
            EmbedToken = embedConfig.EmbedToken,
            Title = "Procurement Dashboard"
        };

        return View(viewModel);
    }

    // Production dashboard
    public async Task<IActionResult> Production()
    {
        var reportId = _configuration["PowerBI:ReportId"]; // In real app, use separate report IDs
        var userId = User.Identity?.Name ?? "anonymous";

        var embedConfig = await _powerBIService.GetEmbedConfigAsync(reportId!, userId);

        var viewModel = new PowerBIDashboardViewModel
        {
            ReportId = embedConfig.ReportId,
            EmbedUrl = embedConfig.EmbedUrl,
            EmbedToken = embedConfig.EmbedToken,
            Title = "Production Capacity Dashboard"
        };

        return View(viewModel);
    }
}