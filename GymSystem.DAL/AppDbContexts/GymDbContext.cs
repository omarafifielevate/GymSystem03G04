using GymSystem.DAL.Configurations;
using GymSystem.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace GymSystem.DAL.AppDbContexts
{
    public class GymDbContext : IdentityDbContext<ApplicationUser>
    {
        public GymDbContext(DbContextOptions contextOptions) : base(contextOptions)
        {
            
        }

        public DbSet<Plan> Plans { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<HealthRecord> HealthRecords { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            //modelBuilder.Entity<ApplicationUser>().ToTable("Users", "identity");
            //modelBuilder.Entity<ApplicationUser>(u =>
            //{
            //    u.Property(au => au.FirstName).HasMaxLength(50);
            //    u.Property(au => au.LastName).HasMaxLength(50);
            //    u.Ignore(au => au.)
            //});

        }


    }
}
