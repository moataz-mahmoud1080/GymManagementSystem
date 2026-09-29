using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagementSystem.BLL.Service.Interfaces;
using GymManagementSystem.BLL.ViewModels.Trainer;
using GymManagementSystem.DAL.Data.Models;
using GymManagementSystem.DAL.Repository.InterFases;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.BLL.Service.Classes
{
    public class TrainerService : ITrainerService
    {
        private readonly IUnitOfWorkRepository _unitOfWork;

        public TrainerService(IUnitOfWorkRepository unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default)
        {
            var trainers = await _unitOfWork.GetRepository<Trainer>().GetAllAsync(ct:ct);

            if (!trainers.Any()) return [];

            var trainerViewModels = trainers.Select(t => new TrainerViewModel
            {
                Id = t.Id,
                Name = t.Name,
                Email = t.Email,
                Phone = t.Phone,
                Specialization =t.Specialtie.ToString()
            });

            return trainerViewModels;
        }

        public async Task<bool> CreateTrainerAsync(CreateTrairerViewModel model, CancellationToken ct = default)
        {
            var existEmail = await _unitOfWork.GetRepository<Trainer>().AnyAsync(m => m.Email == model.Email, ct: ct);
            var existPhone = await _unitOfWork.GetRepository<Trainer>().AnyAsync(m => m.Phone == model.Phone, ct: ct);
            if (existEmail || existPhone) return false;

            var trainer = new Trainer
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                Address = new Address
                {
                    BuildingNo = model.BuildingNumber,
                    City = model.City,
                    Street = model.Street
                },
                Specialtie = model.Specialtie
            };

            try
            {
                _unitOfWork.GetRepository<Trainer>().Add(trainer);
                var result = await _unitOfWork.SaveChangesAsync(ct);
                return result > 0;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        public async Task<TrainerViewModel?> TrainerDetailsAsync(int id, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, ct: ct);
            if(trainer == null) return null;

            var trainerViewModel = new TrainerViewModel
            {
                Id = trainer.Id,
                Name = trainer.Name,
                Email = trainer.Email,
                Phone = trainer.Phone,
                Specialization = $"{trainer.Specialtie}",
                Address= $"{trainer.Address.Street} {trainer.Address.BuildingNo} - {trainer.Address.City}"
            };

            return trainerViewModel;

        }

        public async Task<UpdateTrainerViewModel> GetUpdateTrainerAsync(int id, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, ct: ct);

            if (trainer is null) return null ;

            return new UpdateTrainerViewModel
            {
                Name = trainer.Name,
                Email = trainer.Email,
                Phone = trainer.Phone,
                DateOfBirth = trainer.DateOfBirth,
                Gender = trainer.Gender,
                BuildingNumber = trainer.Address.BuildingNo,
                City = trainer.Address.City,
                Street =trainer.Address.Street,
                Specialtie = trainer.Specialtie

            };         
        }

        public async Task<bool> UpdateTrainerAsync(int id, UpdateTrainerViewModel model, CancellationToken ct = default)
        {
            var trainer =await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, ct: ct);   
            if(trainer is null) return false;

            var emailExists = await _unitOfWork.GetRepository<Trainer>().AnyAsync(e => e.Email == model.Email && e.Id != id, ct: ct);
            var phoneExists = await _unitOfWork.GetRepository<Trainer>().AnyAsync(e => e.Phone == model.Phone && e.Id != id, ct: ct);
            if (emailExists || phoneExists) return false;

            trainer.Email = model.Email;
            trainer.Phone = model.Phone;
            trainer.Specialtie = model.Specialtie;
            trainer.Address.BuildingNo = model.BuildingNumber;
            trainer.Address.City = model.City;
            trainer.Address.Street = model.Street;
            trainer.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Trainer>().Update(trainer);
            var result= await _unitOfWork.SaveChangesAsync(ct);
            return result> 0;


        }

        public async Task<bool> DeleteTrainerAsync(int id, CancellationToken ct = default)
        {
            var trainer =await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, ct: ct);
            if (trainer is null) return false;

            var hasActiveSessions = await _unitOfWork.GetRepository<Session>().AnyAsync(s => s.Id==id && s.EndDate>DateTime.Now ,ct:ct);
            if (hasActiveSessions) return false;

            _unitOfWork.GetRepository<Trainer>().Delete(trainer);
            var result= await _unitOfWork.SaveChangesAsync(ct);
            return result> 0;


        }
    }
}
