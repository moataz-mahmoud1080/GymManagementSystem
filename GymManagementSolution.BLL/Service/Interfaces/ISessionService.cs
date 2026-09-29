using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.ViewModels.Session;

namespace GymManagementSystem.BLL.Service.Interfaces
{
    public interface ISessionService
    {
        Task<IEnumerable<SessionViewModels?>> GetAllSessionsAsync(CancellationToken ct=default);
        Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct = default);
        Task<Result<SessionViewModels>?> GetSessionByIdAsync(int sessionId, CancellationToken ct = default);

        Task<Result<UpdateSessionViewModel>> GetSessionToUpdateByIdAsync(int id,CancellationToken ct=default);
        Task<Result> UpdateSessionAsync(int id,UpdateSessionViewModel model,CancellationToken ct=default);

        Task<Result> DeleteSessionAsync(int id,CancellationToken ct=default);

        Task<IEnumerable<GetCategoryName>> getCategoryNamesAsync(CancellationToken ct = default);
        Task<IEnumerable<GetTrainarsName>> getTrainarsNamesAsync(CancellationToken ct = default);


    }
}
