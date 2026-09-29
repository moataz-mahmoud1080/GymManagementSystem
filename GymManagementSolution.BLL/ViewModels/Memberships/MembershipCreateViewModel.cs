using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagementSystem.BLL.ViewModels.Memberships
{
    public class MembershipCreateViewModel
    {
        [Required]
        public int MemberId { get; set; }

        [Required]
        public int PlanId { get; set; }

        [Required]
        public DateTime StartDate { get; set; } = DateTime.Now;

        // للـ DropDowns في الـ View
        public IEnumerable<SelectListItem>? Members { get; set; }
        public IEnumerable<SelectListItem>? Plans { get; set; }
    }

    public class MembershipViewModel
    {
        public int MemberId { get; set; }
        public string MemberName { get; set; } = string.Empty;
        public int PlanId { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status => EndDate > DateTime.Now ? "Active" : "Expired";
    }
}
