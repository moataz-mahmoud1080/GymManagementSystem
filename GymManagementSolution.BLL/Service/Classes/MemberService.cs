using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GymManagementSystem.BLL.Service.Attstchment;
using GymManagementSystem.BLL.Service.Interfaces;
using GymManagementSystem.BLL.ViewModels.Member;
using GymManagementSystem.DAL.Data.Models;
using GymManagementSystem.DAL.Repository.InterFases;

namespace GymManagementSystem.BLL.Service.Classes
{
    public class MemberService : IMemberService
    {
        private readonly IUnitOfWorkRepository _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAttatchmentService _attatchment;

        public MemberService(IUnitOfWorkRepository unitOfWork ,IMapper mapper , IAttatchmentService attatchment)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _attatchment = attatchment;
        }

        public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default)
        {
            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync(ct: ct);
            if (!members.Any()) return [];

            ///var memberViewModels = members.Select(m => new MemberViewModel
            ///{
            ///    Id = m.Id,
            ///    Photo = m.Photo,
            ///    Name = m.Name,
            ///    Email = m.Email,
            ///    Phone = m.Phone,
            ///    Gender = m.Gender.ToString()
            ///});
            ///
            
            var memberViewModels = _mapper.Map<IEnumerable<MemberViewModel>>(members);
            return memberViewModels;
        }

        public async Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {
            var emailExists = await _unitOfWork.GetRepository<Member>().AnyAsync(m => m.Email == model.Email, ct: ct);
            var phoneExists = await _unitOfWork.GetRepository<Member>().AnyAsync(m => m.Phone == model.Phone, ct: ct);

            if (emailExists || phoneExists) return false;

            var storedPhoto = await _attatchment.UplodeAsync(model.PhotoFile.OpenReadStream(), model.PhotoFile.FileName, "MembersPhoto", ct);
            if (string.IsNullOrWhiteSpace(storedPhoto)) return false;

            var member = _mapper.Map<Member>(model);
            member.Photo = storedPhoto;

            _unitOfWork.GetRepository<Member>().Add(member);
            var result = await _unitOfWork.SaveChangesAsync(ct: ct);

            if (result > 0)
            {
                return true;
            }
            else
            {
                // حذف الصورة لو فشلت عملية الحفظ في الداتا بيز
                _attatchment.Delete(storedPhoto, "MembersPhoto");
                return false;
            }
        }
        public async Task<MemberViewModel?> MemberDetailsAync(int id, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(id, ct: ct);
            if (member is null) return null;

            ///var memberViewModels = new MemberViewModel
            ///{
            ///    Name = member.Name,
            ///    Phone = member.Phone,
            ///    Email = member.Email,
            ///    Gender = member.Gender.ToString(),
            ///    DateOfBirth = member.DateOfBirth.ToShortDateString(),
            ///    Address = $"{member.Address.BuildingNo} - {member.Address.Street} - {member.Address.City}",
            ///};
            ///

 
            var memberViewModels = _mapper.Map<MemberViewModel>(member);

            var activeMember= await _unitOfWork.GetRepository<Membership>().FirstOrDefultAsync(mp=>mp.Id == id &&
                                mp.EndDate > DateTime.Now,ct:ct);

            if (activeMember is not null)
            {
                var activePlan=await _unitOfWork.GetRepository<Plan>().GetByIdAsync(activeMember.Id, ct: ct);
               
                memberViewModels.PlanName=activePlan?.Name;
                memberViewModels.MembershipStartDate = activeMember.CreatedAt.ToString();
                memberViewModels.MembershipEndtDate = activeMember.EndDate.ToString();
            }
            return memberViewModels;
        }

        public async Task<HealthRecordViewModel> GetMemberHealthRecordAsync(int id, CancellationToken ct = default)
        {
            var recour = await _unitOfWork.GetRepository<HealthRecord>().FirstOrDefultAsync(h=>h.MemberId== id,ct:ct);

            ///    Weight = recour.Weight,
            ///return new HealthRecordViewModel
            ///{
            ///    Height = recour.Height,
            ///    BloodType = recour.BloodType,
            ///    Note = recour.Note
            ///};

            return recour is null ? null : _mapper.Map<HealthRecordViewModel>(recour);                    
        }

        public async Task<MemberToUpdateViewModel?> GetMemberToUpdateAsync(int id, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(id, ct: ct);
            return member is null ? null : _mapper.Map<MemberToUpdateViewModel>(member);

            //if (member is null) return null;
            //else
            //    return new MemberToUpdateViewModel
            //    {
            //        Name = member.Name,
            //        Phone = member.Phone,
            //        Email = member.Email,
            //        City = member.Address.City,
            //        BuildingNumber = member.Address.BuildingNo,
            //        Street = member.Address.Street
            //    };
        }
        
        public async Task<bool> UpdateMemberDetailsAsunc( int id, MemberToUpdateViewModel model, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(id, ct: ct);

            if(member is null) return false;

            var emailExists = await _unitOfWork.GetRepository<Member>().AnyAsync(e => e.Email == model.Email && e.Id != id, ct: ct);
            var phoneExists = await _unitOfWork.GetRepository<Member>().AnyAsync(e => e.Phone == model.Phone && e.Id != id, ct: ct);

            if (emailExists || phoneExists) return false;


            //member.Email = model.Email;
            //member.Phone=model.Phone;
            //member.Address.City = model.City;
            //member.Address.BuildingNo = model.BuildingNumber;
            //member.Address.Street=model.Street;
            //member.UpdatedAt = DateTime.Now;

            _mapper.Map(model, member);
            _unitOfWork.GetRepository<Member>().Update(member);
            var result = await _unitOfWork.SaveChangesAsync(ct: ct);
            return result > 0 ;
        }

        public async Task<bool> DeleteMemberAsync(int id, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(id, ct: ct);

            if (member is null) return false;

            var hasFutureSession = await _unitOfWork.GetRepository<Booking>()
                .AnyAsync(b => b.MemberId == id && b.Session.StartDate > DateTime.Now, ct: ct);

            if (hasFutureSession) return false;

            // 1. احفظ اسم الصورة قبل مسح الـ Entity
            var photoName = member.Photo;

            // 2. احذف العضو من الداتا بيز
            _unitOfWork.GetRepository<Member>().Delete(member);
            var result = await _unitOfWork.SaveChangesAsync(ct: ct);

            // 3. لو المسح من الداتا بيز نجح، احذف الصورة من الفولدر
            if (result > 0 && !string.IsNullOrEmpty(photoName))
            {
                _attatchment.Delete(photoName, "MembersPhoto");
                return true;
            }

            return result > 0;
        }

      
    }
}
