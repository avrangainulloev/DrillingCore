using DrillingCore.Core.Entities;
using Microsoft.EntityFrameworkCore;

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

        public DbSet<ProjectGroup> ProjectGroups { get; set; }
        public DbSet<Participant> Participants { get; set; }
        // Добавьте DbSet для других сущностей

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Пример seed-данных: создаём пользователя "admin" (пароль = "admin")
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    PasswordHash = "admin", // НЕ ИСПОЛЬЗУЙТЕ в продакшене!
                    Role = "Administrator"
                }
            );

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
