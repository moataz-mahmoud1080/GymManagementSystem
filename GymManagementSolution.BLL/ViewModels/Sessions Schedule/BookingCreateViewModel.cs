using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagementSystem.BLL.ViewModels.Sessions_Schedule
{
    public class BookingCreateViewModel
    {
        [Required(ErrorMessage = "Please select a member")]
        public int MemberId { get; set; }

        [Required(ErrorMessage = "Please select a session")]
        public int SessionId { get; set; }

        public IEnumerable<SelectListItem>? Members { get; set; }
        public IEnumerable<SelectListItem>? Sessions { get; set; }
    }
}
