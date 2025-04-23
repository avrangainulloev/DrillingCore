using DrillingCore.Core.Entities;
using DrillingCore.Domain.Entities;
using DrillingCore.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
namespace DrillingCore.Infrastructure.Persistence
{
    public class DrillingCoreDbContext : DbContext
    {
        public DrillingCoreDbContext(DbContextOptions<DrillingCoreDbContext> options)
            : base(options)
        {
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<ProjectStatus> ProjectStatuses { get; set; }

        public DbSet<ProjectGroup> ProjectGroups { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Equipment> Equipments { get; set; }
        public DbSet<EquipmentType> EquipmentTypes { get; set; }
        public DbSet<ParticipantEquipment> ParticipantEquipments { get; set; }
        public DbSet<FormType> FormTypes { get; set; }
        public DbSet<ProjectForm> ProjectForms { get; set; }
        public DbSet<ChecklistItem> ChecklistItems { get; set; }
        public DbSet<FormChecklistResponse> FormChecklistResponses { get; set; }
        public DbSet<FormPhoto> FormPhotos { get; set; }

        public DbSet<FormParticipant> FormParticipants { get; set; }
        public DbSet<FormTypeEquipmentType> FormTypeEquipmentTypes { get; set; }

        public DbSet<FormSignature> FormSignatures { get; set; }
        public DbSet<FLHAHazardGroup> FLHAHazardGroups { get; set; }
        public DbSet<FLHAHazard> FLHAHazards { get; set; }
        public DbSet<FLHAFormHazard> FLHAFormHazards { get; set; }
        public DbSet<FLHAForm> FLHAForms { get; set; }
        public DbSet<FormDeliveryRule> FormDeliveryRules { get; set; } = default!;
        public DbSet<FormDeliveryRecipient> FormDeliveryRecipients { get; set; } = default!;
        public DbSet<DrillingForm> DrillingForms { get; set; }  

        // Добавьте DbSet для других сущностей

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new DrillingFormConfiguration());
            modelBuilder.Entity<FormParticipant>()
                .HasOne(fp => fp.Participant)
                .WithMany()
                .HasForeignKey(fp => fp.ParticipantId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<FormSignature>()
    .HasOne(f => f.Participant)
    .WithMany()
    .HasForeignKey(f => f.ParticipantId)
    .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<FormSignature>()
    .HasOne(fs => fs.ProjectForm)
    .WithMany(pf => pf.FormSignatures)
    .HasForeignKey(fs => fs.ProjectFormId);

            modelBuilder.Entity<Participant>()
            .HasOne(p => p.Project)
            .WithMany(p => p.Participants) //если есть
            .HasForeignKey(p => p.ProjectId);

            modelBuilder.Entity<FormTypeEquipmentType>()
    .HasKey(ft => new { ft.FormTypeId, ft.EquipmentTypeId });






            // Пример User
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Id).UseIdentityByDefaultColumn();

                // связь
                entity.HasOne(u => u.Role)
                      .WithMany()
                      .HasForeignKey(u => u.RoleId);


            });


            // Пример для свойства StartDate
            modelBuilder.Entity<Project>()
                .Property(p => p.StartDate)
                .HasConversion(
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc), // Преобразование при сохранении
                    v => v // Обратное преобразование (можно оставить без изменений)
                );

            // Если EndDate может быть NULL, то можно добавить аналогичный конвертер с проверкой:
            modelBuilder.Entity<Project>()
                .Property(p => p.EndDate)
                .HasConversion(
                    v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v,
                    v => v
                );

            modelBuilder.Entity<ProjectGroup>()
       .HasMany(g => g.Participants)
       .WithOne()  // Если в Participant нет свойства для ProjectGroup, иначе укажите его
       .HasForeignKey(p => p.GroupId);

            modelBuilder.Entity<Participant>()
                .HasOne(p => p.User)
                .WithMany()  // Если в User нет коллекции участников, иначе укажите коллекцию
                .HasForeignKey(p => p.UserId);



            modelBuilder.Entity<Project>()
       .HasOne(p => p.Status)
       .WithMany(s => s.Projects)
       .HasForeignKey(p => p.StatusId)
       .OnDelete(DeleteBehavior.Restrict); // или другой подходящий вариант

            modelBuilder.Entity<FormDeliveryRule>()
      .HasMany(r => r.Recipients)
      .WithOne(r => r.FormDeliveryRule)
      .HasForeignKey(r => r.FormDeliveryRuleId);



           
        }




    }
}
