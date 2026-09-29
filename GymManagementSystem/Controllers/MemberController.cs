using System.Runtime.CompilerServices;
using GymManagementSystem.BLL.Service.Attstchment;
using GymManagementSystem.BLL.Service.Interfaces;
using GymManagementSystem.BLL.ViewModels.Member;
using GymManagementSystem.DAL.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementSystem.PL.Controllers
{
    [Authorize(Roles ="SuperAdmin")]
    public class MemberController : Controller
    {
        private readonly IMemberService _memberService;
        private readonly IAttatchmentService _attatchment;

        public MemberController(IMemberService memberService,IAttatchmentService attatchment)
        {
            _memberService = memberService;
            _attatchment = attatchment;
        }

        // Index => GET: MemberController 
        public async Task<IActionResult> Index()
        {
            //انا شيلت ال ct من هنا علشان كان عليها اكسبشن
            var members = await _memberService.GetAllMembersAsync();
            return View(members);
        }


        //Create
        [HttpGet]
        public IActionResult Create() => View();
        [HttpPost]
        public async Task<IActionResult> CreateMember(CreateMemberViewModel model, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return View(nameof(Create), model);


            var result = await _memberService.CreateMemberAsync(model, ct);
            if (result)
                TempData["SuccessMessage"] = "Member created successfully.";
            else
                TempData["ErrorMessage"] = "Failed to create member.";

            return RedirectToAction(nameof(Index));
        }


        //MemberDitails
        public async Task<IActionResult> MemberDetails(int id, CancellationToken ct = default)
        {
            var member = await _memberService.MemberDetailsAync(id, ct);

            //If Member Null Return Index Action With Error Massege

            if (member is null)
            {
                TempData["ErrorMessage"] = "Member Not Found";
                return RedirectToAction(nameof(Index));
            }

            return View(member);

        }


        //MemberHealthRecord
        public async Task<IActionResult> HealthRecordDetails(int id, CancellationToken ct = default)
        {
            var record = await _memberService.GetMemberHealthRecordAsync(id, ct);

            if (record is null)
            {
                TempData["ErrorMessage"] = "Member Not Found";
                return RedirectToAction(nameof(Index));
            }

            return View(record);
        }


        public async Task<IActionResult> Picture(int id)
        {
            var member = await _memberService.MemberDetailsAync(id);
            if (member is null || string.IsNullOrWhiteSpace(member.Photo))
                return NotFound();

            var result = _attatchment.GetFile(member.Photo, "MembersPhoto");
            if (result is null)
                return NotFound();

            return File(result.Value.stream, result.Value.countantType);
        }
        //public async Task<IActionResult> Picture(int id)
        //{
        //    var member = await _memberService.MemberDetailsAync(id);
        //    if(member is null || string.IsNullOrWhiteSpace(member.Photo)) return NotFound();
        //    var result = _attatchment.GetFile(member.Photo, "MembersPhoto");
        //    if(result is null) return NotFound();
        //    return File(result.Value.stream, result.Value.countantType);
        //}



        //MemberEdit

        [HttpGet]
        public async Task<IActionResult> EditMember(int id, CancellationToken ct = default)
        {
            var member = await _memberService.GetMemberToUpdateAsync(id, ct);

            if (member is null)
            {
                TempData["ErrorMessage"] = "Member Not Found";
                return RedirectToAction(nameof(Index));
            }

            return View(member);
        }

        [HttpPost]
        public async Task<IActionResult> EditMember([FromRoute] int id, MemberToUpdateViewModel model, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return View(model);

            var member = await _memberService.UpdateMemberDetailsAsunc(id, model, ct);
            if (member)
                TempData["SuccessMessage"] = "Member Update successfully.";
            else
                TempData["ErrorMessage"] = "Failed to Update member.";

            return RedirectToAction(nameof(Index));
        }

        //Delete Member
        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
        {
            var member = await _memberService.MemberDetailsAync(id, ct);

            if (member is null)
            {
                TempData["ErrorMessage"] = "Member Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromRoute]int id,CancellationToken ct)
        {
            var result = await _memberService.DeleteMemberAsync(id, ct);

            if (result)
                TempData["SuccessMessage"] = "Member Deleted successfully.";
            else
                TempData["ErrorMessage"] = "Failed to Deleted member.";

            return RedirectToAction(nameof(Index));



        }
    }
}
