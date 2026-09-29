using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.ViewModels.AccountLogin
{
    public class AcountViewModel
    {
        [Required(ErrorMessage ="The Email Is Required")]
        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "The Password Is Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }=default!;
        public bool RememberMe { get; set; }
    }
}
