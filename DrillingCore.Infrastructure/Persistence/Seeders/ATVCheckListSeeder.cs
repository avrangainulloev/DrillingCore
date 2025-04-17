using DrillingCore.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace DrillingCore.Infrastructure.Persistence.Seeders
{
    public static class ATVCheckListSeeder
    {
        public static async Task SeedAsync(DrillingCoreDbContext context)
        {
            var atvFormType = await context.FormTypes.FirstOrDefaultAsync(f => f.Name == "ATV/UTV Inspection");
            if (atvFormType != null && !context.ChecklistItems.Any(c => c.FormTypeId == atvFormType.Id))
            {
                var atvChecklistItems = new List<ChecklistItem>
                {
                    new() { FormTypeId = atvFormType.Id, Label = "Brakes (Foot and Hand)", GroupName = "Mechanical" },
                    new() { FormTypeId = atvFormType.Id, Label = "Lights (Headlights, Tail, Brake)", GroupName = "Mechanical" },
                    new() { FormTypeId = atvFormType.Id, Label = "Mirrors", GroupName = "Mechanical" },
                    new() { FormTypeId = atvFormType.Id, Label = "Tires (Wear, Pressure)", GroupName = "Mechanical" },
                    new() { FormTypeId = atvFormType.Id, Label = "Horn", GroupName = "Mechanical" },
                    new() { FormTypeId = atvFormType.Id, Label = "Throttle Operation", GroupName = "Mechanical" },
                    new() { FormTypeId = atvFormType.Id, Label = "Steering", GroupName = "Mechanical" },
                    new() { FormTypeId = atvFormType.Id, Label = "Drive Chain / Shaft", GroupName = "Mechanical" },

                    new() { FormTypeId = atvFormType.Id, Label = "First Aid Kit", GroupName = "Safety Equipment" },
                    new() { FormTypeId = atvFormType.Id, Label = "Fire Extinguisher", GroupName = "Safety Equipment" },
                    new() { FormTypeId = atvFormType.Id, Label = "Reflective Tape / Decals", GroupName = "Safety Equipment" },
                    new() { FormTypeId = atvFormType.Id, Label = "Rollover Protection", GroupName = "Safety Equipment" },
                    new() { FormTypeId = atvFormType.Id, Label = "Seat Belts", GroupName = "Safety Equipment" },
                    new() { FormTypeId = atvFormType.Id, Label = "Backup Alarm", GroupName = "Safety Equipment" },

                    new() { FormTypeId = atvFormType.Id, Label = "Cleanliness", GroupName = "General" },
                    new() { FormTypeId = atvFormType.Id, Label = "No Fluid Leaks", GroupName = "General" },
                    new() { FormTypeId = atvFormType.Id, Label = "Load Secured", GroupName = "General" }
                };

                context.ChecklistItems.AddRange(atvChecklistItems);
                await context.SaveChangesAsync();
            }
        }
    }
}
