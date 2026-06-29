using AutoMapper;
using GymSystem.BLL.Contracts;
using GymSystem.BLL.Results;
using GymSystem.BLL.ViewModels;
using GymSystem.DAL.Contracts;
using GymSystem.DAL.Models;
using GymSystem.DAL.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Services
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SessionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct)
        {

            if(model.StartDate > model.EndDate) return Result.Validation("StartDate Must Be Before EndDate");
            if(model.StartDate < DateTime.Now) return Result.Validation("StartDate Must Be in the future");
            if (model.Capacity > 25 || model.Capacity < 1) return Result.Validation("Capacity Must Be Between 1 and 25");

            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId, ct);
            if(trainer == null) return Result.Validation("Trainer Was Not Found");

            var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(model.CategoryId, ct);
            if (category == null) Result.Validation("Category Was Not Found");

            var isValid = Enum.TryParse<Speciality>(category.Name, true, out var speciality);
            if(!isValid || trainer.Speciality != speciality) return Result.Validation("Trainer Does,'t match the speciality");

            var session = _mapper.Map<Session>(model);

            _unitOfWork.GetRepository<Session>().Add(session);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Session Creation Failed");
        }

        public async Task<Result<IEnumerable<SessionViewModel?>>> GetAllSessionsAsync(CancellationToken ct)
        {
            var sessionRepo = _unitOfWork.SessionRepository;
            var sessions = await sessionRepo.GetAllSessionsWithTrainerAndCategoryAsync(ct);
            if (sessions == null) return Result<IEnumerable<SessionViewModel>>.NotFound("Sessions Not Found");

            var mappedSessions = _mapper.Map<IEnumerable<SessionViewModel>>(sessions);

            // N + 1 !!!!!!!!!!!!!!
            foreach (var session in mappedSessions)
            {
                session.AvailableSlots = session.Capacity - await sessionRepo.GetCountOfBookedSlotsAsync(session.Id, ct);
            }

            return Result<IEnumerable<SessionViewModel>>.Ok(mappedSessions);
        }

        public async Task<IEnumerable<CategorySelectViewModel>> GetCategoriesForDropDownListAsync(CancellationToken ct)
        {
            var result = await _unitOfWork.GetRepository<Category>().GetAllAsync(ct);
            return _mapper.Map<IEnumerable<CategorySelectViewModel>>(result);
        }

        public async Task<IEnumerable<TrainerSelectViewModel>> GetTrainersForDropDownListAsync(CancellationToken ct)
        {
            var result = await _unitOfWork.GetRepository<Trainer>().GetAllAsync(ct);
            return _mapper.Map<IEnumerable<TrainerSelectViewModel>>(result);
        }
    }
}
