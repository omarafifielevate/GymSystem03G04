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

        private readonly IGenericRepository<Member> _memberRepo;
        private readonly IGenericRepository<Membership> _membershipRepo;
        private readonly IGenericRepository<Plan> _planRepo;
        private readonly IGenericRepository<HealthRecord> _healthRecordRepo;
        private readonly IGenericRepository<Booking> _bookingRepo;

        public MemberService(IGenericRepository<Member> membersRepo, IGenericRepository<Membership> membershipRepo, IGenericRepository<Plan> planRepo, IGenericRepository<HealthRecord> healthRecordRepo, IGenericRepository<Booking> bookingRepo)
        {
            _memberRepo = membersRepo;
            _membershipRepo = membershipRepo;
            _planRepo = planRepo;
            _healthRecordRepo = healthRecordRepo;
            _bookingRepo = bookingRepo;
        }

        public async Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct)
        {
            //Validate Email Doesn't Exist
            var emailExists = await _memberRepo.AnyAsync(m => m.Email == model.Email, ct);
            //Validate PHone Doesn't Exist
            var phoneExists = await _memberRepo.AnyAsync(m => m.Phone == model.Phone, ct);

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


            var result = await _memberRepo.AddAsync(member, ct);

            return result > 0;

        }

        public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct)
        {
            var members = await _memberRepo.GetAllAsync(ct);

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

        public async Task<MemberDetailsViewModel?> GetMemberDetailsByIdAsync(int id, CancellationToken ct)
        {
            //Get member from Db

            var member = await _memberRepo.GetByIdAsync(id, ct);

            //if null return null

            if (member == null) return null;

            //map member Entity To MemberDetails View Model

            var memberDetailsVM = new MemberDetailsViewModel
            {
                Id = member.Id,
                Name = member.Name,
                Email = member.Email,
                Gender = member.Gender,
                Phone = member.Phone,
                DateOfBirth = member.DateofBirth,
                Photo = member.Photo,
                Address = $"{member.Address.BuildingNumber}-{member.Address.Street}-{member.Address.City}"
            };

            //active membership

            var activeMembership = await _membershipRepo.FirstOrDefaultAsync(x => x.MemberId == member.Id && x.EndDate > DateTime.Now, ct);

            if(activeMembership != null)
            {
                //PlAN name
                var plan = await _planRepo.GetByIdAsync(activeMembership.PlanId, ct);
                memberDetailsVM.PlanName = plan.Name;
                //assign membershipStartDate & membershipEndDate
                memberDetailsVM.MembershipStartDate = activeMembership.StartDate.ToString();
                memberDetailsVM.MembershipEndDate = activeMembership.EndDate.ToString();
            }


            //return MemberDetailsViewModel

            return memberDetailsVM;
        }

        public async Task<HealthRecordViewModel?> GetMemberHealthRecordAsync(int memberId, CancellationToken ct)
        {
            var healthRecord = await _healthRecordRepo.FirstOrDefaultAsync(x => x.MemberId == memberId, ct);
            if(healthRecord == null) return null;
            return new HealthRecordViewModel
            {
                Weight = healthRecord.Wieght,
                Height = healthRecord.Hieght,
                BloodType = healthRecord.BloodType,
                Notes = healthRecord.Notes,
            };
        }

        public async Task<MemberToUpdateViewModel?> GetMemberToUpdateAsync(int memberId, CancellationToken ct)
        {
            var member = await _memberRepo.GetByIdAsync(memberId, ct);
            if(member == null) return null;
            return new MemberToUpdateViewModel
            {
                Name = member.Name,
                Phone = member.Phone,
                Email = member.Email,
                Photo = member.Photo,
                Street = member.Address.Street,
                City = member.Address.City,
                BuildingNumber = member.Address.BuildingNumber
            };
        }

        public async Task<bool> RemoveMemberAsync(int id, CancellationToken ct)
        {
            var member = await _memberRepo.GetByIdAsync(id, ct);
            if(member == null) return false;

            var hasFutureBookings = await _bookingRepo.AnyAsync(x => x.MemberId == id && x.BookingDate > DateTime.Now, ct);
            if(hasFutureBookings) return false;

            var result = await _memberRepo.DeleteAsync(member, ct);
            return result > 0;
        }

        public async Task<bool> UpdateMemberAsync(int id, MemberToUpdateViewModel model, CancellationToken ct)
        {
            var member = await _memberRepo.GetByIdAsync(id, ct);
            if(member == null) return false;

            var emailExists = await _memberRepo.AnyAsync(x => x.Email == model.Email && x.Id != model.Id, ct);
            var PhoneExists = await _memberRepo.AnyAsync(x => x.Phone == model.Phone && x.Id != model.Id, ct);

            if(emailExists || PhoneExists) return false;
            
            member.Email = model.Email;
            member.Phone = model.Phone;
            member.Address.Street = model.Street;
            member.Address.City = model.City;
            member.Address.BuildingNumber = model.BuildingNumber;
            member.UpdatedAt = DateTime.Now;

            var result = await _memberRepo.UpdateAsync(member, ct);
            return result > 0;
        }
    }
}
