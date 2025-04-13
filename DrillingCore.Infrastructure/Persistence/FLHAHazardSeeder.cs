using DrillingCore.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Infrastructure.Persistence
{
    public static class FLHAHazardSeeder
    {
        public static async Task SeedAsync(DrillingCoreDbContext context)
        {
            if (!await context.FLHAHazardGroups.AnyAsync())
            {
                var groups = new List<FLHAHazardGroup>
            {
                new() { Id = 1, Name = "Builders" },
                new() { Id = 2, Name = "Drillers" }
            };
                context.FLHAHazardGroups.AddRange(groups);
            }

            if (!await context.FLHAHazards.AnyAsync())
            {
                var hazards = new List<FLHAHazard>
                {
                    new() { Label = "Overhead Loads", ControlSuggestion = "Wear hard hats, avoid standing under load", GroupId = 2 },
                    new() { Label = "Manual Lifting", ControlSuggestion = "Lift with legs, ask for help", GroupId = 2 },
                    new() { Label = "Slippery Surfaces", ControlSuggestion = "Use anti-slip mats, wear grip boots", GroupId = 2 },
                    new() { Label = "Rotating Equipment", ControlSuggestion = "Stay clear of moving parts, follow lockout procedures", GroupId = 2 },
                    new() { Label = "Pressurized Lines", ControlSuggestion = "Check for leaks, use shields, follow depressurization steps", GroupId = 2 },
                    new() { Label = "High Noise Levels", ControlSuggestion = "Wear ear protection", GroupId = 2 },
                    new() { Label = "Confined Spaces", ControlSuggestion = "Obtain entry permit, continuous gas monitoring", GroupId = 2 },
                    new() { Label = "Chemical Exposure", ControlSuggestion = "Wear gloves, goggles, follow MSDS", GroupId = 2 },
                    new() { Label = "Dropped Objects", ControlSuggestion = "Use tool lanyards, avoid walking under work areas", GroupId = 2 },
                    new() { Label = "Night Operations", ControlSuggestion = "Ensure proper lighting, wear high-vis gear", GroupId = 2 },
                    new() { Label = "Fatigue", ControlSuggestion = "Follow work/rest schedule, stay hydrated", GroupId = 2 },
                    new() { Label = "High-Pressure Pumps", ControlSuggestion = "Ensure guards in place, monitor gauges regularly", GroupId = 2 },
                    new() { Label = "Weather Exposure", ControlSuggestion = "Dress appropriately, monitor for signs of heat/cold stress", GroupId = 2 }
                };
                context.FLHAHazards.AddRange(hazards);
            }

            await context.SaveChangesAsync();
        }
    }
}
