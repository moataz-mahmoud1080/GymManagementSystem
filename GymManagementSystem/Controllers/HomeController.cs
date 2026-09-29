using System.Diagnostics;
using System.Threading.Tasks;
using GymManagementSystem.BLL.Service.Interfaces;
using GymManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAnalyticsService _analytics;

        public HomeController(ILogger<HomeController> logger , IAnalyticsService analytics)
        {
            _logger = logger;
            _analytics = analytics;
        }

        public async Task<IActionResult> Index(CancellationToken ct =default)
        {
            var date = await _analytics.GetAnalyticsAsync(ct);
            return View(date);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
