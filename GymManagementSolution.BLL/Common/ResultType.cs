using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Common
{
    public enum ResultType
    {
        Ok,
        NotFound,
        ValidationFailed,
        Conflict,
        Forbidden
    }
}
