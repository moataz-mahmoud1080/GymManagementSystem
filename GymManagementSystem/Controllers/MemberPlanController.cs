using GymManagementSystem.BLL.ViewModels.Memberships;
using GymManagementSystem.DAL.Data.Models;
using GymManagementSystem.DAL.Repository.InterFases;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagementSystem.PL.Controllers
{
    public class MemberPlanController : Controller
    {
        private readonly IUnitOfWorkRepository _unitOfWork;

        public MemberPlanController(IUnitOfWorkRepository unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: /MemberPlan/Index
        public async Task<IActionResult> Index()
        {
            var memberships = await _unitOfWork.GetRepository<Membership>().GetAllAsync();

            // تحويل الـ Entities إلى ViewModels لعرض الأسماء والحالة بشكل منظم في الـ View
            var viewModel = memberships.Select(m => new MembershipViewModel
            {
                MemberId = m.MemberId,
                MemberName = m.Member?.Name ?? "N/A",
                PlanId = m.PlanId,
                PlanName = m.Plan?.Name ?? "N/A",
                StartDate = m.CreatedAt ?? DateTime.Now,
                EndDate = m.EndDate
            });

            return View(viewModel);
        }

        // GET: /MemberPlan/Create
        public async Task<IActionResult> Create()
        {
            // جلب كافة الخطط وتصفيتها بالـ LINQ في الذاكرة لتحديد الخطط النشطة فقط
            var allPlans = await _unitOfWork.GetRepository<Plan>().GetAllAsync();
            var activePlans = allPlans.Where(p => p.IsActive);

            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync();

            var viewModel = new MembershipCreateViewModel
            {
                Plans = activePlans.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name }),
                Members = members.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Name })
            };

            return View(viewModel);
        }

        // POST: /MemberPlan/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MembershipCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // إعادة ملء القوائم المنسدلة في حال وجود خطأ في البيانات المدخلة
                var allPlans = await _unitOfWork.GetRepository<Plan>().GetAllAsync();
                model.Plans = allPlans.Where(p => p.IsActive).Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name });

                var members = await _unitOfWork.GetRepository<Member>().GetAllAsync();
                model.Members = members.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Name });

                return View(model);
            }

            // Rule: التحقق من عدم وجود اشتراك نشط للعضو بـ FirstOrDefultAsync المتاحة لديك
            var existingActive = await _unitOfWork.GetRepository<Membership>()
                .FirstOrDefultAsync(m => m.MemberId == model.MemberId && m.EndDate > DateTime.Now);

            if (existingActive != null)
            {
                ModelState.AddModelError("", "This member already has an active membership.");

                var allPlans = await _unitOfWork.GetRepository<Plan>().GetAllAsync();
                model.Plans = allPlans.Where(p => p.IsActive).Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name });
                var members = await _unitOfWork.GetRepository<Member>().GetAllAsync();
                model.Members = members.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Name });

                return View(model);
            }

            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(model.PlanId);
            if (plan == null || !plan.IsActive)
            {
                ModelState.AddModelError("", "Selected plan is invalid or inactive.");
                return View(model);
            }

            // إنشاء الكيان وحساب تاريخ الانتهاء تلقائياً
            var membership = new Membership
            {
                MemberId = model.MemberId,
                PlanId = model.PlanId,
                CreatedAt = model.StartDate,
                EndDate = model.StartDate.AddDays(plan.DurationDays)
            };

            _unitOfWork.GetRepository<Membership>().Add(membership);
            await _unitOfWork.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: /MemberPlan/Cancel
        [HttpPost]
        public async Task<IActionResult> Cancel(int memberId, int planId)
        {
            var membership = await _unitOfWork.GetRepository<Membership>()
                .FirstOrDefultAsync(m => m.MemberId == memberId && m.PlanId == planId && m.EndDate > DateTime.Now);

            if (membership != null)
            {
                _unitOfWork.GetRepository<Membership>().Delete(membership);
                await _unitOfWork.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}