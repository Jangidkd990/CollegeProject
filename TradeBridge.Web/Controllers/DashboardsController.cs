namespace TradeBridge.Web.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class DashboardsController : Controller
{
    public IActionResult Index()
    {
        return RedirectToAction("Index", "Dashboard");
    }

    public IActionResult Sales()
    {
        return RedirectToAction("Index", "Dashboard");
    }

    public IActionResult Procurement() 
    {
        return RedirectToAction("Index", "Dashboard");
    }

    public IActionResult Production()
    {
        return RedirectToAction("Index", "Dashboard");
    }
}