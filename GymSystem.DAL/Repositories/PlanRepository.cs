using GymSystem.DAL.AppDbContexts;
using GymSystem.DAL.Contracts;
using GymSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL.Repositories
{
    public class PlanRepository : IPlanRepository
    {
        private readonly GymDbContext _dbContext;

        public PlanRepository(GymDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> AddAsync(Plan plan, CancellationToken ct = default)
        {
            _dbContext.Add(plan);
            return await _dbContext.SaveChangesAsync(ct);
        }

        public async Task<int> DeleteAsync(Plan plan, CancellationToken ct = default)
        {
            _dbContext.Plans.Remove(plan);
            return await _dbContext.SaveChangesAsync(ct);
        }

        public async Task<IEnumerable<Plan>> GetAllAsync(bool isTracked = false, CancellationToken ct = default)
        {
            IQueryable<Plan> plans = isTracked ? _dbContext.Plans : _dbContext.Plans.AsNoTracking();
            //Felteration
            //await plans.Where(p => p.Price > 500).Select(p => p.Name).ToListAsync();
            return await plans.ToListAsync(ct);
        }

        public async Task<Plan> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _dbContext.Plans.FindAsync(id, ct);
        }
            
        public async Task<int> UpdateAsync(Plan plan, CancellationToken ct = default)
        {
            _dbContext.Plans.Update(plan);
            return await _dbContext.SaveChangesAsync(ct);
        }
    }
}
