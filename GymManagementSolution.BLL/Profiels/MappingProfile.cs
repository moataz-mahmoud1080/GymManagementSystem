using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GymManagementSystem.BLL.ViewModels.Member;
using GymManagementSystem.BLL.ViewModels.Session;
using GymManagementSystem.DAL.Data.Models;

namespace GymManagementSystem.BLL.Profiels
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            MemberMaping();
            SessionMapping();

        }
        private void MemberMaping()
        {

            // 1. Member -> MemberViewModel
            CreateMap<Member, MemberViewModel>()
                .ForMember(g => g.Gender, n => n.MapFrom(m => m.Gender))
                .ForMember(a => a.DateOfBirth, n => n.MapFrom(m => m.DateOfBirth.ToShortDateString()))
                .ForMember(a => a.Address, n => n.MapFrom(member => $"{member.Address.BuildingNo} - {member.Address.Street} - {member.Address.City}"));

            // 2. HealthRecord <-> HealthRecordViewModel
            CreateMap<HealthRecord, HealthRecordViewModel>().ReverseMap();

            // 3. CreateMemberViewModel -> Member
            CreateMap<CreateMemberViewModel, Member>()
                .ForMember(m => m.Address, opt => opt.MapFrom(src => new Address
                {
                    BuildingNo = src.BuildingNumber,
                    Street = src.Street,
                    City = src.City
                }))
                .ForMember(h => h.HealthRecord, o => o.MapFrom(s => s.HealthRecordViewModel));

            // Member -> MemberToUpdateViewModel (لشاشة التعديل Get)
            CreateMap<Member, MemberToUpdateViewModel>()
                .ForMember(d => d.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(d => d.Photo, opt => opt.MapFrom(src => src.Photo))
                .AfterMap((s, d) =>
                {
                    if (s.Address != null)
                    {
                        d.BuildingNumber = s.Address.BuildingNo;
                        d.Street = s.Address.Street;
                        d.City = s.Address.City;
                    }
                });

            // MemberToUpdateViewModel -> Member (عند الحفظ Save/Update)
            CreateMap<MemberToUpdateViewModel, Member>()
                .ForMember(dest => dest.Name, opt => opt.Ignore())  // لمنع تغيير الاسم في الداتابيز
                .ForMember(dest => dest.Photo, opt => opt.Ignore()) // لمنع تغيير الصورة في الداتابيز
                .ForMember(m => m.Address, opt => opt.MapFrom(src => new Address
                {
                    BuildingNo = src.BuildingNumber,
                    Street = src.Street,
                    City = src.City
                }))
                .AfterMap((src, dest) =>
                {
                    dest.UpdatedAt = DateTime.Now;
                });
        }

        private void SessionMapping()
        {                   // 1. Session -> SessionViewModel
             CreateMap<Session, SessionViewModels>()
                    .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                    .ForMember(dest => dest.TrainerName, opt => opt.MapFrom(src => src.Trainer.Name))
                    .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName))
                    .ForMember(dest => dest.AvailableSlots, opt => opt.Ignore());

             CreateMap<CreateSessionViewModel, Session>();

                     // 3. Dropdowns Mapping
             CreateMap<Trainer, GetTrainarsName>()
                     .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                     .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

            CreateMap<Category, GetCategoryName>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                    .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.CategoryName));


            //Session <==>  UpdateSessionViewModel
            CreateMap<Session, UpdateSessionViewModel>().ReverseMap();    

        
        }
    }
}
