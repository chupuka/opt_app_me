using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProForm.Models;

namespace ProForm.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<MembershipType> MembershipTypes { get; set; }
        public DbSet<Membership> Memberships { get; set; }
        public DbSet<MembershipFreeze> MembershipFreezes { get; set; }
        public DbSet<FitnessClass> FitnessClasses { get; set; }
        public DbSet<ClassRegistration> ClassRegistrations { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<TrainerSchedule> TrainerSchedules { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Настройка связей
            builder.Entity<Membership>()
                .HasOne(m => m.Client)
                .WithMany(c => c.Memberships)
                .HasForeignKey(m => m.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Membership>()
                .HasOne(m => m.MembershipType)
                .WithMany(mt => mt.Memberships)
                .HasForeignKey(m => m.MembershipTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<MembershipFreeze>()
                .HasOne(f => f.Membership)
                .WithMany(m => m.Freezes)
                .HasForeignKey(f => f.MembershipId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<FitnessClass>()
                .HasOne(c => c.Trainer)
                .WithMany(t => t.Classes)
                .HasForeignKey(c => c.TrainerId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<ClassRegistration>()
                .HasOne(r => r.Class)
                .WithMany(c => c.Registrations)
                .HasForeignKey(r => r.ClassId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ClassRegistration>()
                .HasOne(r => r.Client)
                .WithMany(c => c.ClassRegistrations)
                .HasForeignKey(r => r.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Payment>()
                .HasOne(p => p.Client)
                .WithMany(c => c.Payments)
                .HasForeignKey(p => p.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Payment>()
                .HasOne(p => p.Membership)
                .WithMany()
                .HasForeignKey(p => p.MembershipId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<TrainerSchedule>()
                .HasOne(s => s.Trainer)
                .WithMany(t => t.Schedules)
                .HasForeignKey(s => s.TrainerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Индексы для производительности
            builder.Entity<Client>()
                .HasIndex(c => c.Phone)
                .IsUnique();

            builder.Entity<Trainer>()
                .HasIndex(t => t.Email)
                .IsUnique();

            builder.Entity<FitnessClass>()
                .HasIndex(c => c.StartTime);

            builder.Entity<Membership>()
                .HasIndex(m => new { m.ClientId, m.IsActive });
        }
    }
}

