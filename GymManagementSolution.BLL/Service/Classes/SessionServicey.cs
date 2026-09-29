using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.Service.Interfaces;
using GymManagementSystem.BLL.ViewModels.Session;
using GymManagementSystem.DAL.Data.Models;
using GymManagementSystem.DAL.Data.Models.Enums;
using GymManagementSystem.DAL.Repository.Classes;
using GymManagementSystem.DAL.Repository.InterFases;
using static System.Collections.Specialized.BitVector32;

namespace GymManagementSystem.BLL.Service.Classes
{
    public class SessionServicey : ISessionService
    {
        private readonly IUnitOfWorkRepository _unitOfWork;
        private readonly IMapper _mapper;

        public SessionServicey(IUnitOfWorkRepository unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<IEnumerable<SessionViewModels?>> GetAllSessionsAsync(CancellationToken ct = default)
        {
            var sessions = await _unitOfWork._sessionRepository.GetAllSessionsWithTrainarAndCategoryAsync(ct);
            if (sessions == null || !sessions.Any()) return null;
            var sessionViewModels = _mapper.Map<IEnumerable<SessionViewModels>>(sessions);

            ///var sessionViewModels = sessions.Select(s => new SessionViewModels
            ///{
            ///    Id = s.Id,
            ///    Description = s.Description,
            ///    Capacity = s.Capacity,
            ///    StartDate = s.StarttDate,
            ///    EndDate = s.EndDate,
            ///    TrainerName = s.Trainer.Name ,
            ///    CategoryName = s.Category.CategoryName,
            ///});

            foreach (var session in sessionViewModels)
                session.AvailableSlots = session.Capacity - await _unitOfWork._sessionRepository.GetCountOfBookedSlotsAsync(session.Id, ct);

            return sessionViewModels;
        }

        public async Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct = default)
        {
            if (model.EndDate <= model.StartDate)
                return Result.Validation("End Date Must Be After Start Date");

            if (model.StartDate < DateTime.Now)
                return Result.Validation("Can not edit a session That has Already Staarted");

            if (model.Capacity <= 0 || model.Capacity > 25)
                return Result.Validation("Capacity Must Be Between 1 And 25");

            var trainar = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId);
            if (trainar == null)
                return Result.NotFound("Id Of Trainar Not Found");

            var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(model.CategoryId);
            if (category == null)
                return Result.NotFound("Category Not Found ");



            var isValid = Enum.TryParse<Specialtie>(category.CategoryName, out var CategorySpecialty);

            if (!isValid || trainar.Specialtie != CategorySpecialty)
                return Result.Validation("Can not Create This Session For This Trainer Category Does not Match Specialties");

            var session = _mapper.Map<Session>(model);

            _unitOfWork.GetRepository<Session>().Add(session);

            var result = await _unitOfWork.SaveChangesAsync(ct);

            return result > 0? Result.Ok() : Result.Fail("Failed To Create Session");
        }

        public async Task<Result<SessionViewModels>?> GetSessionByIdAsync(int sessionId, CancellationToken ct = default)
        {

            var session = await _unitOfWork._sessionRepository.GetSessionDetailsWithTrainerAndCategoryAsync(sessionId, ct);

            if (session is null) return Result<SessionViewModels>.NotFound("Session Not Found");
            else
            {
                var mappedSession = _mapper.Map<SessionViewModels>(session);
                mappedSession.AvailableSlots = mappedSession.Capacity - (await
                _unitOfWork._sessionRepository.GetCountOfBookedSlotsAsync(session.Id, ct)); 

                  return Result<SessionViewModels>.Ok(mappedSession);
            }

        }
        public async Task<Result<UpdateSessionViewModel>> GetSessionToUpdateByIdAsync(int id, CancellationToken ct = default)
        {
            var result = await _unitOfWork._sessionRepository.GetByIdAsync(id);
            if (result == null) return Result<UpdateSessionViewModel>.NotFound("Session Not Found");


            if (result.StartDate <= DateTime.Now)
                return Result< UpdateSessionViewModel>.Validation("Can not edit a session That has Already Staarted");

            var bookCount = await _unitOfWork._sessionRepository.GetCountOfBookedSlotsAsync(id);
            if (bookCount > 0)
                return Result<UpdateSessionViewModel>.NotFound("Cannot Edit Session That Has Booknig ");
            
            return Result<UpdateSessionViewModel>.Ok(_mapper.Map<UpdateSessionViewModel>(result));

        }
        public async Task<Result> UpdateSessionAsync(int id, UpdateSessionViewModel model, CancellationToken ct = default)
        {
            var result = await _unitOfWork._sessionRepository.GetByIdAsync(id);
            if (result == null) return Result.NotFound("Session Not Found");

            if (result.StartDate <= DateTime.Now)
                return Result.Validation("Can not edit a session That has Already Staarted");

            var bookCount = await _unitOfWork._sessionRepository.GetCountOfBookedSlotsAsync(id);
            if (bookCount > 0)
                return Result.NotFound("Cannot Edit Session That Has Booknig ");

            //check model 

            if (model.StartDate < DateTime.Now)
                return Result.Validation("Can not edit a session That has Already Staarted");

            if (model.EndDate <= model.StartDate)
                return Result.Validation("End Date Must Be After Start Date");

            var trainar = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId);
            if (trainar == null)
                return Result.NotFound("Id Of Trainar Not Found");

            var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(result.CategoryId);
            if (category == null)
                return Result.NotFound("Category Not Found ");


            var isValid = Enum.TryParse<Specialtie>(category.CategoryName, out var CategorySpecialty);

            if (!isValid || trainar.Specialtie != CategorySpecialty) 
                return Result.Validation("Can not Create This Session For This Trainer Category Does not Match Specialties");


            _mapper.Map(model, result);
            result.UpdatedAt = DateTime.Now;
            _unitOfWork._sessionRepository.Update(result);  

            var resultSaveChanges = await _unitOfWork.SaveChangesAsync();

            return resultSaveChanges > 0 ? Result.Ok() : Result.Fail("Failed To Update Session");
        }
        public async Task<Result> DeleteSessionAsync(int id, CancellationToken ct = default)
        {
            var session =await _unitOfWork._sessionRepository.GetByIdAsync(id);
            if (session == null) return Result.NotFound("Session NOT Found");

            if (session.EndDate >= DateTime.Now)
                return Result.Fail("Cannot Delete Session That Has Not Ended Yet");

            var bookCount= await _unitOfWork._sessionRepository.GetCountOfBookedSlotsAsync(id);
            if (bookCount > 0)
                return Result.NotFound("Cannot Delete Session That Has Booknig ");

            _unitOfWork._sessionRepository.Delete(session);
            var result = await _unitOfWork.SaveChangesAsync();

            return result > 0 ? Result.Ok() : Result.Fail("Failed To Delete Session ");
        }
        public async Task<IEnumerable<GetCategoryName>> getCategoryNamesAsync(CancellationToken ct = default)
        {
            var categories = await _unitOfWork.GetRepository<Category>().GetAllAsync(ct: ct);
            return _mapper.Map<IEnumerable<GetCategoryName>>(categories);
        }
        public async Task<IEnumerable<GetTrainarsName>> getTrainarsNamesAsync(CancellationToken ct = default)
        {
            var trainars = await _unitOfWork.GetRepository<Trainer>().GetAllAsync(ct: ct);
            return _mapper.Map<IEnumerable<GetTrainarsName>>(trainars);
        }

    }
}
