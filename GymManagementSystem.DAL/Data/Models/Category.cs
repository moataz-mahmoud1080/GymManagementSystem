using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.DAL.Data.Models
{
    public class Category : BaseClass
    {
        public string CategoryName { get; set; } = default!;

        public ICollection<Session> Sessions { get; set; } = new HashSet<Session>();

    }
}
