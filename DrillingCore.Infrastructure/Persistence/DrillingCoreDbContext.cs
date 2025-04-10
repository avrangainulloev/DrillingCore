using DrillingCore.Core.Entities;
using DrillingCore.Domain.Entities;
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

        // Добавьте DbSet для других сущностей

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FormParticipant>()
                .HasOne(fp => fp.Participant)
                .WithMany()
                .HasForeignKey(fp => fp.ParticipantId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FormSignature>()
    .HasOne(fs => fs.ProjectForm)
    .WithMany(pf => pf.FormSignatures)
    .HasForeignKey(fs => fs.ProjectFormId);

            modelBuilder.Entity<Participant>()
            .HasOne(p => p.Project)
            .WithMany() // или .WithMany(p => p.Participants) если есть
            .HasForeignKey(p => p.ProjectId);

            modelBuilder.Entity<FormTypeEquipmentType>()
    .HasKey(ft => new { ft.FormTypeId, ft.EquipmentTypeId });


            modelBuilder.Entity<FormType>().HasData(
    new FormType { Id = 2, Name = "Drill Inspection" }
);

            modelBuilder.Entity<EquipmentType>().HasData(
                new EquipmentType { Id = 1, TypeName = "Drill" }
            );

            modelBuilder.Entity<FormTypeEquipmentType>().HasData(
                new FormTypeEquipmentType { FormTypeId = 2, EquipmentTypeId = 1 } // DrillInspection — Drill
                                                                                  // добавь другие при необходимости
            );

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Roles");
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Id).UseIdentityByDefaultColumn();

                entity.HasData(
                    new Role { Id = -1, Name = "Administrator" },
                    new Role { Id = -2, Name = "Driller" },
                    new Role { Id = -3, Name = "ProjectManager" }
                );
            });

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

                entity.HasData(
                    new User
                    {
                        Id = 1,
                        Username = "admin",
                        PasswordHash = "admin",
                        RoleId = -1, // должно совпадать с Role { Id = -1 }
                        FullName = "Administrator",
                        Email = "admin@example.com",
                        Mobile = "1234567890"
                    }
                );
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

            // Можно задать начальное заполнение (seed) для статусов, если нужно:
            modelBuilder.Entity<ProjectStatus>().HasData(
                new ProjectStatus { Id = 1, Name = "Active", Description = "Проект активный" },
                new ProjectStatus { Id = 2, Name = "Inactive", Description = "Проект не активный" },
                new ProjectStatus { Id = 3, Name = "Suspended", Description = "Проект приостановлен" },
                new ProjectStatus { Id = 4, Name = "Completed", Description = "Проект завершён" }
            );

        }




    }
}
