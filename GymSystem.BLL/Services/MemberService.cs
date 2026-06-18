using GymSystem.BLL.Contracts;
using GymSystem.BLL.ViewModels;
using GymSystem.DAL.Contracts;
using GymSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Services
{
    public class MemberService : IMemberService
    {

        private readonly IGenericRepository<Member> _membersRepo;

        public MemberService(IGenericRepository<Member> membersRepo)
        {
            _membersRepo = membersRepo;
        }

        public async Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct)
        {
            //Validate Email Doesn't Exist
            var emailExists = await _membersRepo.AnyAsync(m => m.Email == model.Email, ct);
            //Validate PHone Doesn't Exist
            var phoneExists = await _membersRepo.AnyAsync(m => m.Phone == model.Phone, ct);

            if(emailExists || phoneExists) return false;

            var member = new Member
            {
                Email = model.Email,
                Phone = model.Phone,
                Name = model.Name,
                Gender = model.Gender,
                DateofBirth = model.DateOfBirth,
                Address = new Address
                {
                    City = model.City,
                    Street = model.Street,
                    BuildingNumber = model.BuildingNumber
                },
                HealthRecord = new HealthRecord
                {
                    Hieght = model.HealthRecordViewModel.Height,
                    Wieght = model.HealthRecordViewModel.Weight,
                    BloodType = model.HealthRecordViewModel.BloodType,
                    Notes = model.HealthRecordViewModel?.Notes
                }
            };


            var result = await _membersRepo.AddAsync(member, ct);

            return result > 0;

        }

        public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct)
        {
            var members = await _membersRepo.GetAllAsync(ct);

            if (!members.Any()) return [];

            var memberViewModels = members.Select(m => new MemberViewModel
            {
                Id = m.Id,
                Name = m.Name,
                Email = m.Email,
                Gender = m.Gender,
                Phone = m.Phone,
                Photo = m.Photo,
            });

            return memberViewModels;
        }
    }
}
