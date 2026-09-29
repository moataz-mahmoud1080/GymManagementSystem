using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagementSystem.DAL.Data.Models.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GymManagementSystem.BLL.ViewModels.Member
{
    public class MemberViewModel
    {
        public int Id { get; set; }
        public string? Photo { get; set; }
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string Gender { get; set; } = default!;


        //Member DETAILE

        public string? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? PlanName { get; set; }
        public string? MembershipStartDate { get; set; }
        public string? MembershipEndtDate { get; set; }



    }
}
