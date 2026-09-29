using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper.Execution;
using GymManagementSystem.BLL.Service.Interfaces;
using GymManagementSystem.BLL.ViewModels.Analytics;
using GymManagementSystem.DAL.Data.Models;
using GymManagementSystem.DAL.Repository.InterFases;

namespace GymManagementSystem.BLL.Service.Classes
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IUnitOfWorkRepository _unitOfWork;

        public AnalyticsService(IUnitOfWorkRepository unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<AnalyticsViewModel> GetAnalyticsAsync(CancellationToken ct = default)
        {
            var now=DateTime.Now;
            var UpcomingSession = await _unitOfWork._sessionRepository.CountAsync(s=>s.StartDate <= now);
            var OngoingSessions =await _unitOfWork._sessionRepository.CountAsync(s=>s.StartDate <= now && now <= s.EndDate);
            var CompletedSessions = await _unitOfWork._sessionRepository.CountAsync(s => s.EndDate < now);

            var TotalMembers = await _unitOfWork.GetRepository<DAL.Data.Models.Member>().CountAsync(ct: ct);
            var TotalTrainers = await _unitOfWork.GetRepository<Trainer>().CountAsync(ct: ct);
            var ActiveMember =await _unitOfWork.GetRepository<Membership>().CountAsync(c=>c.EndDate>now ,ct);

            return new AnalyticsViewModel
            {
                UpcomingSessions=UpcomingSession,
                OngoingSessions=OngoingSessions,
                CompletedSessions=CompletedSessions,
                TotalMembers=TotalMembers,
                TotalTrainers=TotalTrainers,
                ActiveMembers=ActiveMember

            };

        }
    }
}
