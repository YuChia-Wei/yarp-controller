using Microsoft.AspNetCore.Mvc;

namespace Yarp.ControlPlant.WebApi.Controllers;

/// <summary>
/// 首頁轉址
/// </summary>
public class HomeController : Controller
{
    // GET
    /// <summary>
    /// index
    /// </summary>
    /// <returns></returns>
    public IActionResult Index()
    {
        return new RedirectResult("swagger");
    }
}