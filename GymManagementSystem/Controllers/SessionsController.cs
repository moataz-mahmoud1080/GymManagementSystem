using System.Threading.Tasks;
using GymManagementSystem.BLL.Service.Classes;
using GymManagementSystem.BLL.Service.Interfaces;
using GymManagementSystem.BLL.ViewModels.Session;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagementSystem.PL.Controllers
{
    [Authorize]
    public class SessionsController : Controller
    {
        private readonly ISessionService _session;

        public SessionsController(ISessionService session)
        {
            _session = session;
        }
        public async Task<IActionResult> Index(CancellationToken ct = default)
        {
            var sessions = await _session.GetAllSessionsAsync(ct);
            return View(sessions);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await DashPord();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateSessionViewModel model, CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
            {
               await DashPord();
                return View(model);
            }

            var result = await _session.CreateSessionAsync(model, ct);
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Session created successfully.";
                return RedirectToAction(nameof(Index));
            }

                TempData["ErrorMessage"] = "Failed to create session.";
                await DashPord();
                return View(model);
            
        }
        [HttpGet]
        public async Task<IActionResult> Detiles(int id, CancellationToken ct = default)
        {
            var result = await _session.GetSessionByIdAsync(id, ct);
            if (result.IsSuccess)
            {
                return View(result.Value);
            }
            else
            {
                TempData["ErrorMessage"] = result.Error;
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct = default)
        {
            var result = await _session.GetSessionToUpdateByIdAsync(id, ct);
            if (result.IsSuccess)
            {
                var trainers = await _session.getTrainarsNamesAsync(ct);
                ViewBag.Trainers = new SelectList(trainers, "Id", "Name");

                return View(result.Value);

            }
            TempData["ErrorMessage"] = result.Error;
            return RedirectToAction(nameof(Index));
        }
        
        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdateSessionViewModel model, CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
            {
                var trainers = await _session.getTrainarsNamesAsync(ct);

                ViewBag.Trainers = new SelectList(trainers, "Id", "Name", model.TrainerId);
                return View(model);
            }

            var result = await _session.UpdateSessionAsync(id, model, ct);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Session Updated Successfuly";
                return RedirectToAction(nameof(Index));
            }

            // Update failed because of business rule
            TempData["ErrorMessage"] = result.Error;

            var trainersAfterError = await _session.getTrainarsNamesAsync(ct);

            ViewBag.Trainers = new SelectList(trainersAfterError, "Id", "Name", model.TrainerId);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
        {
            var result = await _session.GetSessionByIdAsync(id, ct);

            if (result.IsSuccess)
            {
                return View(result.Value);

            }
            else
            {
                TempData["ErrorMessage"] = result.Error;
                return RedirectToAction(nameof(Index));              
            }         

        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct = default)
        {
            var session = await _session.DeleteSessionAsync(id, ct);

            TempData[session.IsSuccess ? "SuccessMessage" : "ErrorMessage"]
                = session.IsSuccess ? "Session  Deleted  Successfuly" : session.Error;

            return RedirectToAction(nameof(Index));
        }



        private async Task DashPord(CancellationToken ct = default)
        {
            var categories = await _session.getCategoryNamesAsync(ct);
            var trainers = await _session.getTrainarsNamesAsync(ct);

            ViewBag.Categories = new SelectList(categories, "Id", "CategoryName");
            ViewBag.Trainers = new SelectList(trainers, "Id", "Name");
        }
    }
}
