using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagementSystem.BLL.ViewModels.Trainer;

namespace GymManagementSystem.BLL.Service.Interfaces
{
    public interface ITrainerService
    {
        Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default);
        Task<bool> CreateTrainerAsync(CreateTrairerViewModel model, CancellationToken ct = default);
        Task<TrainerViewModel?> TrainerDetailsAsync(int id, CancellationToken ct = default);
        Task<UpdateTrainerViewModel> GetUpdateTrainerAsync(int id, CancellationToken ct = default);
        Task<bool> UpdateTrainerAsync(int id, UpdateTrainerViewModel model, CancellationToken ct = default);

        Task<bool> DeleteTrainerAsync(int id, CancellationToken ct = default);

    }
}
