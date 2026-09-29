using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagementSystem.DAL.Data.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.DAL.Data.Models
{
    public abstract class GymUser : BaseClass
    {
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public DateOnly DateOfBirth { get; set; }
        public Gender Gender { get; set; }

        public Address Address { get; set; }

    }
    [Owned]
    public class Address
    {
        public int BuildingNo { get; set; } = default!;
        public string Street { get; set; } = default!;
        public string City { get; set; } = default!;
    }
}
