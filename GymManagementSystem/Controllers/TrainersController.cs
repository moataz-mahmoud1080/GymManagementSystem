using System.Threading.Tasks;
using GymManagementSystem.BLL.Service.Interfaces;
using GymManagementSystem.BLL.ViewModels.Trainer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.PL.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class TrainersController : Controller
    {
        private readonly ITrainerService _trainer;

        public TrainersController(ITrainerService trainer)
        {
            _trainer = trainer;
        }
        public async Task<IActionResult> Index()
        {
            var trainers = await _trainer.GetAllTrainersAsync();
            return View(trainers);
        }


        [HttpGet]
        public IActionResult Create() => View();
        [HttpPost]
        public async Task<IActionResult> Create(CreateTrairerViewModel model, CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _trainer.CreateTrainerAsync(model, ct);
            if (result)
            {
                TempData["SuccessMessage"] = "Trainer created successfully.";
                return RedirectToAction(nameof(Index));
            }

            // في حالة الفشل: ابقى في نفس الصفحة واعرض الخطأ
            ModelState.AddModelError(string.Empty, "Failed to create trainer. Email or Phone might already exist or format is invalid.");
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct = default)
        {
            var trainer = await _trainer.TrainerDetailsAsync(id, ct);
            if (trainer == null)
            {
                return NotFound();
            }
            return View(trainer);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var trainer = await _trainer.GetUpdateTrainerAsync(id,ct);

            if (trainer == null)
            {
                TempData["ErrorMessage"] = "Trainar Not Found";
                return RedirectToAction(nameof(Index));
            }

            return View(trainer);

        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromRoute] int id, UpdateTrainerViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);
            
            var result = await _trainer.UpdateTrainerAsync(id,model, ct);
            if (!result)
            {
                TempData["ErrorMessage"] = "Trainer Not Updated";
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "Trainer updated successfully.";
            return RedirectToAction(nameof(Index));
        }




        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
        {
            var trainer = await _trainer.TrainerDetailsAsync(id, ct);
            if (trainer == null)
            {
                TempData["ErrorMessage"] = "Trainer Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromRoute] int id, CancellationToken ct = default)
        {
            var result = await _trainer.DeleteTrainerAsync(id, ct);
            if (!result)
            {
                TempData["ErrorMessage"] = "Failed to delete trainer.";
                return RedirectToAction(nameof(Index));
            }
            TempData["SuccessMessage"] = "Trainer deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
