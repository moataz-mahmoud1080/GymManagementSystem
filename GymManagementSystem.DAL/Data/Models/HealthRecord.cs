using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.DAL.Data.Models
{
    public class HealthRecord :BaseClass
    {
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public string BloodType { get; set; }
        public string?  Note { get; set; }

        //LastUpdate => Take from UpdatedAt property of BaseClass

        public Member Member { get; set; } = default!;
        public int? MemberId {  get; set; }

    }
}
