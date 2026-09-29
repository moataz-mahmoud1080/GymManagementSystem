using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.DAL.Data.Models
{
    public class Booking :BaseClass
    {
        public bool IsAttended { get; set; } = false;
        public int MemberId { get; set; }
        public int SessionId { get; set; }
        public Member Member { get; set; } = default!;
        public Session Session { get; set; } = default!;



    }
}
