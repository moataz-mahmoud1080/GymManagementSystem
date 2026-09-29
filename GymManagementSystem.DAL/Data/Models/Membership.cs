using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GymManagementSystem.DAL.Data.Models
{
    public class Membership :BaseClass
    {
        public DateTime EndDate { get; set; }
        public Plan Plan { get; set; } = default!;
        public int PlanId { get; set; }

        public Member Member { get; set; } = default!;
        public int MemberId { get; set; }

        public string Status => EndDate > DateTime.UtcNow ? "Active" : "Expired";
        public bool IsActive => EndDate > DateTime.UtcNow;
    }
}
