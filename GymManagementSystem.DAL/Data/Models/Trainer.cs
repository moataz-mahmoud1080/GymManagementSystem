using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagementSystem.DAL.Data.Models.Enums;

namespace GymManagementSystem.DAL.Data.Models
{
    public class Trainer :GymUser
    {
        public Specialtie Specialtie { get; set; }
        //HireDate → => take from CreatedAt property in BaseClass

        public ICollection<Session> Sessions { get; set; } = default!;

    }
}
