using DrillingCore.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace DrillingCore.Infrastructure.Persistence.Seeders
{
    public static class DrillCheckListSeeder
    {
        public static async Task SeedAsync(DrillingCoreDbContext context)
        {
            var drillFormType = await context.FormTypes.FirstOrDefaultAsync(f => f.Name == "Drill Inspection");

            if (drillFormType != null && !context.ChecklistItems.Any(c => c.FormTypeId == drillFormType.Id))
            {
                var checklistItems = new List<ChecklistItem>
                {
                    new() { FormTypeId = drillFormType.Id, Label = "Cap Mag (away from powder box)", GroupName = "Equipment & Storage" },
                    new() { FormTypeId = drillFormType.Id, Label = "Propane stored upright and away from mags", GroupName = "Equipment & Storage" },
                    new() { FormTypeId = drillFormType.Id, Label = "Hoods Covering Mag Locks", GroupName = "Equipment & Storage" },
                    new() { FormTypeId = drillFormType.Id, Label = "Drive Line \"U\" Joints (Tandem Only)", GroupName = "Equipment & Storage" },
                    new() { FormTypeId = drillFormType.Id, Label = "Light (Front, Back, Mast & Deck)", GroupName = "Equipment & Storage" },
                    new() { FormTypeId = drillFormType.Id, Label = "Seat Belt (3 inch for LIS)", GroupName = "Equipment & Storage" },
                    new() { FormTypeId = drillFormType.Id, Label = "Leaks", GroupName = "Equipment & Storage" },
                    new() { FormTypeId = drillFormType.Id, Label = "Kelly Hose", GroupName = "Equipment & Storage" },
                    new() { FormTypeId = drillFormType.Id, Label = "Cleanliness", GroupName = "Equipment & Storage" },
                    new() { FormTypeId = drillFormType.Id, Label = "Break Out Bowl Ram", GroupName = "Equipment & Storage" },
                    new() { FormTypeId = drillFormType.Id, Label = "Pressure Gauges", GroupName = "Equipment & Storage" },
                    new() { FormTypeId = drillFormType.Id, Label = "Mast Rams", GroupName = "Equipment & Storage" },

                    new() { FormTypeId = drillFormType.Id, Label = "2-way Radio with external speaker", GroupName = "Safety & Accessories" },
                    new() { FormTypeId = drillFormType.Id, Label = "Escape Hutch (LIS Only)", GroupName = "Safety & Accessories" },
                    new() { FormTypeId = drillFormType.Id, Label = "Windows", GroupName = "Safety & Accessories" },
                    new() { FormTypeId = drillFormType.Id, Label = "Pull Down Ram, Cable or chains", GroupName = "Safety & Accessories" },
                    new() { FormTypeId = drillFormType.Id, Label = "Myno Pump", GroupName = "Safety & Accessories" },
                    new() { FormTypeId = drillFormType.Id, Label = "Hydraulic Hoses", GroupName = "Safety & Accessories" },
                    new() { FormTypeId = drillFormType.Id, Label = "Emergency Shut Down Switch", GroupName = "Safety & Accessories" },
                    new() { FormTypeId = drillFormType.Id, Label = "Galvo", GroupName = "Safety & Accessories" },
                    new() { FormTypeId = drillFormType.Id, Label = "4 Brass Knife", GroupName = "Safety & Accessories" },
                    new() { FormTypeId = drillFormType.Id, Label = "E Hoper Stinger Point", GroupName = "Safety & Accessories" },
                    new() { FormTypeId = drillFormType.Id, Label = "First Aid Kit (B.C basic level with book)", GroupName = "Safety & Accessories" },
                    new() { FormTypeId = drillFormType.Id, Label = "Fire Extinguisher (2-20lb BC or 2x 10lb AB)", GroupName = "Safety & Accessories" },
                    new() { FormTypeId = drillFormType.Id, Label = "Regulator on Propane Tank", GroupName = "Safety & Accessories" },
                    new() { FormTypeId = drillFormType.Id, Label = "Explosive Placards, MSDS on board", GroupName = "Safety & Accessories" },
                    new() { FormTypeId = drillFormType.Id, Label = "ERP", GroupName = "Safety & Accessories" },
                    new() { FormTypeId = drillFormType.Id, Label = "Back-Up Alarm", GroupName = "Safety & Accessories" },
                };

                context.ChecklistItems.AddRange(checklistItems);
                await context.SaveChangesAsync();
            }
        }
    }
}
