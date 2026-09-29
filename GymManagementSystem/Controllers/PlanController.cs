using GymManagementSystem.BLL.Service.Classes;
using GymManagementSystem.BLL.Service.Interfaces;
using GymManagementSystem.BLL.ViewModels.Plan;
using GymManagementSystem.DAL.Data.Models;
using GymManagementSystem.DAL.Repository.InterFases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.Controllers
{
    [Authorize]
    public class PlanController : Controller
    {
        private readonly IPlanService _plan;

        public PlanController(IPlanService plan)
        {
            _plan = plan;
        }

        //Get /BaseUrl/Plan/Index
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var plans = await _plan.GetAllPlansAsync(ct);
            return View(plans);
        }

        //Get /BaseUrl/Plan/Details/{Id}
        public async Task<IActionResult> Details(int id)
        {
            var plan = await _plan.PlanDetailsAsync(id);
            if (plan == null)
                return NotFound();

            return View(plan);
        }

        //Edit
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var plan = await _plan.PlanDetailsAsync(id, ct);
            if (plan == null)
                return NotFound();
            return View(plan);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, PlanViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return View(model);
            // Assuming you have a method to update the plan in your service
            var result = await _plan.UpdatePlanAsync(id, model, ct);
            if (!result)
            {
                ModelState.AddModelError("", "Failed to update the plan.");
                return View(model);
            }
            TempData["SuccessMessage"] = "Plan updated successfully.";
            return RedirectToAction(nameof(Index));
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(int id,CancellationToken ct)
        {
            var result = await _plan.TogglePlanStatusAsync(id,ct);

            if (!result)
            {
                TempData["Error"] = "Plan status could not be changed.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "Plan Status Changed";

            return RedirectToAction(nameof(Index));
        }

    }
}
