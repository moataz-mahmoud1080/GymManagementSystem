using GymManagementSystem.BLL.ViewModels.Sessions_Schedule;
using GymManagementSystem.DAL.Data.Models;
using GymManagementSystem.DAL.Repository.InterFases;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagementSystem.PL.Controllers
{
    public class MemberSessionController : Controller
    {
        private readonly IUnitOfWorkRepository _unitOfWork;

        public MemberSessionController(IUnitOfWorkRepository unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: /MemberSession/Index (عرض جدول المواعيد)
        public async Task<IActionResult> Index()
        {
            var sessions = await _unitOfWork.GetRepository<Session>().GetAllAsync();
            var categories = await _unitOfWork.GetRepository<Category>().GetAllAsync();
            var trainers = await _unitOfWork.GetRepository<Trainer>().GetAllAsync();
            var bookings = await _unitOfWork.GetRepository<Booking>().GetAllAsync();

            var viewModel = sessions.Select(s => new SessionScheduleViewModel
            {
                SessionId = s.Id,
                CategoryName = categories.FirstOrDefault(c => c.Id == s.CategoryId)?.CategoryName ?? "General",
                TrainerName = trainers.FirstOrDefault(t => t.Id == s.TrainerId)?.Name ?? "N/A",
                StartDate = s.StartDate,
                EndDate = s.EndDate,
                Capacity = s.Capacity,
                BookedCount = bookings.Count(b => b.SessionId == s.Id)
            }).OrderBy(s => s.StartDate);

            return View(viewModel);
        }

        // GET: /MemberSession/Create (حجز حصة)
        public async Task<IActionResult> Create(int? sessionId)
        {
            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync();
            var sessions = await _unitOfWork.GetRepository<Session>().GetAllAsync();
            var categories = await _unitOfWork.GetRepository<Category>().GetAllAsync();

            // إظهار الجلسات القادمة فقط
            var upcomingSessions = sessions.Where(s => s.StartDate > DateTime.Now);

            var viewModel = new BookingCreateViewModel
            {
                SessionId = sessionId ?? 0,
                Members = members.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Name }),
                Sessions = upcomingSessions.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = $"{categories.FirstOrDefault(c => c.Id == s.CategoryId)?.CategoryName} - ({s.StartDate:ddd, dd MMM hh:mm tt})"
                })
            };

            return View(viewModel);
        }

        // POST: /MemberSession/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await ReloadDropdownsAsync(model);
                return View(model);
            }

            // 1. Rule: التأكد من وجود اشتراك نشط للعضو[cite: 4]
            var activeMembership = await _unitOfWork.GetRepository<Membership>()
                .FirstOrDefultAsync(m => m.MemberId == model.MemberId && m.EndDate > DateTime.Now);

            if (activeMembership == null)
            {
                ModelState.AddModelError("", "Member must have an active membership to book a session.");
                await ReloadDropdownsAsync(model);
                return View(model);
            }

            // 2. Rule: التأكد من سعة وموعد الجلسة[cite: 4]
            var session = await _unitOfWork.GetRepository<Session>().GetByIdAsync(model.SessionId);
            if (session == null || session.StartDate <= DateTime.Now)
            {
                ModelState.AddModelError("", "Cannot book a past or non-existing session.");
                await ReloadDropdownsAsync(model);
                return View(model);
            }

            var sessionBookings = await _unitOfWork.GetRepository<Booking>().GetAllAsync();
            var bookedCount = sessionBookings.Count(b => b.SessionId == model.SessionId);

            if (bookedCount >= session.Capacity)
            {
                ModelState.AddModelError("", "This session is full.");
                await ReloadDropdownsAsync(model);
                return View(model);
            }

            // 3. Rule: عدم تكرار حجز نفس الجلسة لنفس العضو[cite: 4]
            var alreadyBooked = sessionBookings.Any(b => b.MemberId == model.MemberId && b.SessionId == model.SessionId);
            if (alreadyBooked)
            {
                ModelState.AddModelError("", "Member is already booked in this session.");
                await ReloadDropdownsAsync(model);
                return View(model);
            }

            // إنشاء الحجز[cite: 4]
            var booking = new Booking
            {
                MemberId = model.MemberId,
                SessionId = model.SessionId,
                CreatedAt = DateTime.Now,
                IsAttended = false // افتراضياً لم يحضر بعد[cite: 4]
            };

            _unitOfWork.GetRepository<Booking>().Add(booking);
            await _unitOfWork.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task ReloadDropdownsAsync(BookingCreateViewModel model)
        {
            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync();
            var sessions = await _unitOfWork.GetRepository<Session>().GetAllAsync();
            var categories = await _unitOfWork.GetRepository<Category>().GetAllAsync();

            model.Members = members.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Name });
            model.Sessions = sessions.Where(s => s.StartDate > DateTime.Now).Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = $"{categories.FirstOrDefault(c => c.Id == s.CategoryId)?.CategoryName} - ({s.StartDate:ddd, dd MMM hh:mm tt})"
            });
        }
    }
}
