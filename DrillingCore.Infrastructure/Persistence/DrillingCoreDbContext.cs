using DrillingCore.Core.Entities;
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

        public DbSet<ProjectGroup> ProjectGroups { get; set; }
        public DbSet<Participant> Participants { get; set; }
        // Добавьте DbSet для других сущностей

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
        }
    }
}
