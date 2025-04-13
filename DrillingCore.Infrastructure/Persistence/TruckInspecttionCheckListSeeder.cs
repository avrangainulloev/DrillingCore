using DrillingCore.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Infrastructure.Persistence
{
    public static class TruckInspecttionCheckListSeeder
    {
        public static async Task SeedAsync(DrillingCoreDbContext context)
        {
            var truckFormType = await context.FormTypes.FirstOrDefaultAsync(f => f.Name == "Truck Inspection");
            if (truckFormType != null && !context.ChecklistItems.Any(c => c.FormTypeId == truckFormType.Id))
            {
                var truckChecklistItems = new List<ChecklistItem>
                {
                    // Exterior Inspection
                    new() { FormTypeId = truckFormType.Id, Label = "Headlights operational", GroupName = "Exterior Inspection" },
                    new() { FormTypeId = truckFormType.Id, Label = "Turn signals and brake lights working", GroupName = "Exterior Inspection" },
                    new() { FormTypeId = truckFormType.Id, Label = "Windshield clean and undamaged", GroupName = "Exterior Inspection" },
                    new() { FormTypeId = truckFormType.Id, Label = "Mirrors properly adjusted and clean", GroupName = "Exterior Inspection" },
                    new() { FormTypeId = truckFormType.Id, Label = "Tires inflated and no visible damage", GroupName = "Exterior Inspection" },
                    new() { FormTypeId = truckFormType.Id, Label = "No fluid leaks under truck", GroupName = "Exterior Inspection" },
                    new() { FormTypeId = truckFormType.Id, Label = "Body panels and bumpers intact", GroupName = "Exterior Inspection" },
                    new() { FormTypeId = truckFormType.Id, Label = "Mudflaps present and secured", GroupName = "Exterior Inspection" },

                    // Interior & Safety Equipment
                    new() { FormTypeId = truckFormType.Id, Label = "Horn operational", GroupName = "Interior & Safety" },
                    new() { FormTypeId = truckFormType.Id, Label = "Wipers and washer fluid working", GroupName = "Interior & Safety" },
                    new() { FormTypeId = truckFormType.Id, Label = "Seat belts functional", GroupName = "Interior & Safety" },
                    new() { FormTypeId = truckFormType.Id, Label = "Emergency kit present", GroupName = "Interior & Safety" },
                    new() { FormTypeId = truckFormType.Id, Label = "Fire extinguisher charged and accessible", GroupName = "Interior & Safety" },
                    new() { FormTypeId = truckFormType.Id, Label = "No loose objects in cab", GroupName = "Interior & Safety" },
                    new() { FormTypeId = truckFormType.Id, Label = "Dash warning lights checked", GroupName = "Interior & Safety" },

                    // Engine Bay
                    new() { FormTypeId = truckFormType.Id, Label = "Engine oil level OK", GroupName = "Engine Bay" },
                    new() { FormTypeId = truckFormType.Id, Label = "Coolant level OK", GroupName = "Engine Bay" },
                    new() { FormTypeId = truckFormType.Id, Label = "Power steering fluid level OK", GroupName = "Engine Bay" },
                    new() { FormTypeId = truckFormType.Id, Label = "Belts and hoses in good condition", GroupName = "Engine Bay" },
                    new() { FormTypeId = truckFormType.Id, Label = "Battery terminals clean and tight", GroupName = "Engine Bay" },

                    // Suspension & Undercarriage
                    new() { FormTypeId = truckFormType.Id, Label = "No broken or missing leaf springs", GroupName = "Undercarriage" },
                    new() { FormTypeId = truckFormType.Id, Label = "Shock absorbers not leaking", GroupName = "Undercarriage" },
                    new() { FormTypeId = truckFormType.Id, Label = "Frame and crossmembers intact", GroupName = "Undercarriage" },
                    new() { FormTypeId = truckFormType.Id, Label = "Brake lines secure and undamaged", GroupName = "Undercarriage" },

                    // Brakes & Air System
                    new() { FormTypeId = truckFormType.Id, Label = "Service brakes function properly", GroupName = "Brakes & Air System" },
                    new() { FormTypeId = truckFormType.Id, Label = "Parking brake holds", GroupName = "Brakes & Air System" },
                    new() { FormTypeId = truckFormType.Id, Label = "Air pressure builds correctly", GroupName = "Brakes & Air System" },
                    new() { FormTypeId = truckFormType.Id, Label = "No audible air leaks", GroupName = "Brakes & Air System" },
                    new() { FormTypeId = truckFormType.Id, Label = "Air tanks drained", GroupName = "Brakes & Air System" },

                    // Documents
                    new() { FormTypeId = truckFormType.Id, Label = "Registration and insurance present", GroupName = "Documents" },
                    new() { FormTypeId = truckFormType.Id, Label = "Driver’s license valid and on-hand", GroupName = "Documents" },
                    new() { FormTypeId = truckFormType.Id, Label = "Trip inspection log filled", GroupName = "Documents" },

                    // Cargo Area
                    new() { FormTypeId = truckFormType.Id, Label = "Cargo secured", GroupName = "Load & Cargo" },
                    new() { FormTypeId = truckFormType.Id, Label = "No loose items in box or bed", GroupName = "Load & Cargo" },
                    new() { FormTypeId = truckFormType.Id, Label = "Tailgate or doors latched", GroupName = "Load & Cargo" },
                    new() { FormTypeId = truckFormType.Id, Label = "Load within weight limit", GroupName = "Load & Cargo" },
                };

                context.ChecklistItems.AddRange(truckChecklistItems);
                await context.SaveChangesAsync();
            }
        }
    }
}
