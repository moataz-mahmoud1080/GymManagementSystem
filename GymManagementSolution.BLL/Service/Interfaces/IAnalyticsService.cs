using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagementSystem.BLL.ViewModels.Analytics;

namespace GymManagementSystem.BLL.Service.Interfaces
{
    public interface IAnalyticsService
    {
        Task<AnalyticsViewModel> GetAnalyticsAsync(CancellationToken ct = default);

    }
}
