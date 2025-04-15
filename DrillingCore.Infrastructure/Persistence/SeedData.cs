using DrillingCore.Core.Entities;
using DrillingCore.Infrastructure.Persistence.Seeders;
using Microsoft.EntityFrameworkCore;

namespace DrillingCore.Infrastructure.Persistence
{
    public static class SeedData
    {
        public static async Task SeedAsync(DrillingCoreDbContext context)
        {

            if (!context.ProjectStatuses.Any())
            {
                context.ProjectStatuses.AddRange(new[] {
                new ProjectStatus { Name = "Active", Description = "Проект активный" },
                new ProjectStatus { Name = "Inactive", Description = "Проект не активный" },
                new ProjectStatus { Name = "Suspended", Description = "Проект приостановлен" },
                new ProjectStatus { Name = "Completed", Description = "Проект завершён" }
                });
            }
            if (!context.Roles.Any())
            {
                context.Roles.AddRange(new List<Role>
                {
                    new() { Name = "Administrator" },
                    new() { Name = "User" },
                    new() { Name = "ProjectManager" }
                });

                await context.SaveChangesAsync();
            }

            if (!context.Users.Any())
            {
                context.Users.Add(new User
                {

                    Username = "admin",
                    PasswordHash = "admin",
                    RoleId = 1,
                    FullName = "Administrator",
                    Email = "admin@example.com",
                    Mobile = "1234567890"
                });

                await context.SaveChangesAsync();
            }

            if (!context.EquipmentTypes.Any())
            {
                context.EquipmentTypes.AddRange(new List<EquipmentType>
               {
                   new() {TypeName = "Truck"},
                   new() {TypeName = "Drill"},
                   new() {TypeName = "ATV"},
                   new() {TypeName = "DTV"}
               });
                await context.SaveChangesAsync();
            }

            if (!context.FormTypes.Any())
            {
                context.FormTypes.AddRange(new List<FormType>
                {
                    new() {  Name = "Truck Inspection" },
                    new() {  Name = "Drill Inspection" },
                    new() {  Name = "FLHA" },
                    new() {  Name = "Safety Checklist" },
                    new() {  Name = "Well Servicing" }
                });

                await context.SaveChangesAsync();
            }
            if (!context.FormTypeEquipmentTypes.Any())
            {
                context.FormTypeEquipmentTypes.Add(new FormTypeEquipmentType { EquipmentTypeId = 2, FormTypeId = 2 });
                await context.SaveChangesAsync();
            }


            await FLHAHazardSeeder.SeedAsync(context);
            await DrillCheckListSeeder.SeedAsync(context);
            await TruckInspecttionCheckListSeeder.SeedAsync(context);
        }
    }
}
