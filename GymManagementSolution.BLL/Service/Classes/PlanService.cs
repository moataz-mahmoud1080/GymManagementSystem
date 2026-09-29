using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using GymManagementSystem.BLL.Service.Interfaces;
using GymManagementSystem.BLL.ViewModels.Plan;
using GymManagementSystem.DAL.Data.Models;
using GymManagementSystem.DAL.Repository.InterFases;

namespace GymManagementSystem.BLL.Service.Classes
{
    public class PlanService : IPlanService
    {
        private readonly IUnitOfWorkRepository _unitOfWork;

        public PlanService(IUnitOfWorkRepository unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct = default)
        {
            var plan =await _unitOfWork.GetRepository<Plan>().GetAllAsync(ct:ct);
            if (!plan.Any()) return [];

            var planViewModels = plan.Select(p => new PlanViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Duration = p.DurationDays,
                Description = p.Description,
                IsActive = p.IsActive
            });

            return planViewModels;
        }

        public async Task<PlanViewModel?> PlanDetailsAsync(int id, CancellationToken ct = default)
        {
            var plan =await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct:ct);
            if (plan == null) return null;

            return new PlanViewModel
            {
                Name = plan.Name,
                Price = plan.Price,
                Duration = plan.DurationDays,
                Description = plan.Description,
                IsActive = plan.IsActive
            };
        }     

        public async Task<bool> UpdatePlanAsync(int id, PlanViewModel model, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct: ct);
            if (plan == null) return false;

            if(plan.IsActive) return false;


            plan.Description = model.Description;
            plan.DurationDays = model.Duration;
            plan.Price= model.Price;


            _unitOfWork.GetRepository<Plan>().Update(plan);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0;
        }
        public async Task<bool> TogglePlanStatusAsync(int id, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct:ct);

            if (plan is null)
                return false;

            plan.IsActive = !plan.IsActive;

            _unitOfWork.GetRepository<Plan>().Update(plan);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0;
        }
    }
}
