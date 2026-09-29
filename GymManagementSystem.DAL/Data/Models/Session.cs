using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.DAL.Data.Models
{
    public class Session : BaseClass
    {
        public string Description { get; set; } = null!;
        public int Capacity { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Trainer Trainer { get; set; } = default!;
        public int TrainerId { get; set; }

        public Category Category { get; set; } = default!;
        public int CategoryId { get; set; }

        public ICollection<Booking> SessionMembers { get; set; } = default!;
    }
}
