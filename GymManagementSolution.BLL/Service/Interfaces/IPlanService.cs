using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagementSystem.BLL.ViewModels.Plan;

namespace GymManagementSystem.BLL.Service.Interfaces
{
    public interface IPlanService
    {
        Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct = default);
        Task<PlanViewModel?> PlanDetailsAsync(int id, CancellationToken ct = default);
        Task<bool> UpdatePlanAsync (int id, PlanViewModel model, CancellationToken ct = default);
        Task<bool> TogglePlanStatusAsync(int id, CancellationToken ct = default);
    }
}
