using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.ViewModels.Sessions_Schedule
{
    public class SessionScheduleViewModel
    {
        public int SessionId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string TrainerName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Capacity { get; set; }
        public int BookedCount { get; set; }
        public bool IsFull => BookedCount >= Capacity;

        // حالة الجلسة: قادمة، جارية، أو منتهية
        public string Status
        {
            get
            {
                var now = DateTime.Now;
                if (StartDate > now) return "Upcoming";
                if (StartDate <= now && EndDate >= now) return "Ongoing";
                return "Completed";
            }
        }
    }
}
