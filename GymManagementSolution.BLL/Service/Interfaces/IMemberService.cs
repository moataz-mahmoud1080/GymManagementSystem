using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagementSystem.BLL.ViewModels.Member;

namespace GymManagementSystem.BLL.Service.Interfaces
{
    public interface IMemberService
    {
        Task<IEnumerable<MemberViewModel>> GetAllMembersAsync (CancellationToken ct=default);

        Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default);

        Task<MemberViewModel?> MemberDetailsAync(int id, CancellationToken ct = default);

        Task<HealthRecordViewModel> GetMemberHealthRecordAsync(int id, CancellationToken ct = default);

        Task<MemberToUpdateViewModel?> GetMemberToUpdateAsync(int id, CancellationToken ct = default);

        Task<bool> UpdateMemberDetailsAsunc(int id , MemberToUpdateViewModel model ,CancellationToken ct=default);


        Task<bool> DeleteMemberAsync(int id, CancellationToken ct = default);
    }
}
