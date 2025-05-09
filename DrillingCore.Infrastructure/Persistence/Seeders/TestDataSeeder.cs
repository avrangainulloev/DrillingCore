using DrillingCore.Core.Entities;
using DrillingCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Infrastructure.Persistence.Seeders
{
    public static class TestDataSeeder
    {
        public static async Task SeedAsync(DrillingCoreDbContext context)
        {
            if (!await context.Equipments.AnyAsync())
            {
                var equipments = new List<Equipment>
            {
                new() {  Name = "Drill 1", EquipmentTypeId =2, CreatedDate=DateTime.UtcNow, RegistrationNumber="1231" },
                new() {  Name = "Drill 2", EquipmentTypeId =2, CreatedDate=DateTime.UtcNow, RegistrationNumber="1232" },
                new() {  Name = "Drill 3", EquipmentTypeId =2, CreatedDate=DateTime.UtcNow, RegistrationNumber="1233" },
                new() {  Name = "Drill 4", EquipmentTypeId =2, CreatedDate=DateTime.UtcNow, RegistrationNumber="1234" },
                new() {  Name = "Drill 5", EquipmentTypeId =2, CreatedDate=DateTime.UtcNow, RegistrationNumber="1235" },

            };
                context.Equipments.AddRange(equipments);
            }

            if (!await context.Users.AnyAsync())
            {

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

                }
                var fullNames = new List<string>
{
    "Liam Johnson", "Olivia Smith", "Noah Williams", "Emma Brown",
    "Oliver Jones", "Ava Davis", "Elijah Wilson", "Sophia Miller",
    "James Taylor", "Charlotte Anderson", "Benjamin Thomas", "Amelia Moore",
    "Lucas Martin", "Mia Jackson", "Henry White", "Harper Harris",
    "Alexander Thompson", "Evelyn Lewis", "William Young", "Abigail Hall",
    "Logan Allen", "Isabella Wright", "Jackson King", "Emily Scott",
    "Daniel Green", "Scarlett Adams", "Sebastian Baker", "Ella Nelson",
    "Jacob Carter", "Grace Mitchell", "Jack Perez", "Chloe Roberts",
    "Owen Turner", "Lily Phillips", "Matthew Campbell", "Zoe Parker",
    "Nathan Evans", "Victoria Stewart", "Leo Rivera", "Hannah Sanchez"
};

                var users = new List<User>();

                for (int i = 0; i < fullNames.Count; i++)
                {
                    var nameParts = fullNames[i].Split(' ');
                    var firstName = nameParts[0].ToLower();
                    var lastName = nameParts[1].ToLower();

                    users.Add(new User
                    {
                        FullName = fullNames[i],
                        Username = $"{firstName}.{lastName}",
                        PasswordHash = "hashedpassword", // заменить на реальное хэширование при необходимости
                        RoleId = 2,
                        Email = $"{firstName}.{lastName}@example.com",
                        Mobile = $"555-01{i + 1:D2}", // например: 555-0101, 555-0102, ...
                        JobTitle = "Helper",
                        IsActive = true
                    });
                }

            }

            if (!await context.Projects.AnyAsync())
            {
                var projects = new List<Project>
    {
        new() { Name = "Swan Hills Infill Program", Location = "Swan Hills, AB", StartDate = DateTime.UtcNow.AddDays(-30), EndDate = null, Client = "Cenovus Energy", HasCampOrHotel = true, StatusId = 1 },
        new() { Name = "Kaybob Duvernay Pad A", Location = "Fox Creek, AB", StartDate = DateTime.UtcNow.AddDays(-60), EndDate = DateTime.UtcNow.AddDays(15), Client = "Chevron Canada", HasCampOrHotel = true, StatusId = 1 },
        new() { Name = "Fort Hills SAGD Expansion", Location = "Fort McMurray, AB", StartDate = DateTime.UtcNow.AddDays(-120), EndDate = null, Client = "Suncor", HasCampOrHotel = true, StatusId = 1 },
        new() { Name = "Leduc County Waterline Bore", Location = "Leduc, AB", StartDate = DateTime.UtcNow.AddDays(-90), EndDate = DateTime.UtcNow.AddDays(-10), Client = "Alberta Infrastructure", HasCampOrHotel = false, StatusId = 2 },
        new() { Name = "Peace River Horizontal Wells", Location = "Peace River, AB", StartDate = DateTime.UtcNow.AddDays(-75), EndDate = null, Client = "Baytex Energy", HasCampOrHotel = true, StatusId = 1 },
        new() { Name = "Trans Mountain Pump Station", Location = "Kamloops, BC", StartDate = DateTime.UtcNow.AddDays(-150), EndDate = DateTime.UtcNow.AddDays(30), Client = "Trans Mountain Corp", HasCampOrHotel = true, StatusId = 1 },
        new() { Name = "Medicine Hat Geotech Survey", Location = "Medicine Hat, AB", StartDate = DateTime.UtcNow.AddDays(-40), EndDate = null, Client = "City of Medicine Hat", HasCampOrHotel = false, StatusId = 1 },
        new() { Name = "SaskPower Wind Monitoring", Location = "Swift Current, SK", StartDate = DateTime.UtcNow.AddDays(-20), EndDate = null, Client = "SaskPower", HasCampOrHotel = false, StatusId = 1 },
        new() { Name = "Grande Prairie Pad B Drilling", Location = "Grande Prairie, AB", StartDate = DateTime.UtcNow.AddDays(-85), EndDate = null, Client = "Ovintiv", HasCampOrHotel = true, StatusId = 1 },
        new() { Name = "Regina Pipeline Integrity", Location = "Regina, SK", StartDate = DateTime.UtcNow.AddDays(-50), EndDate = DateTime.UtcNow.AddDays(10), Client = "Enbridge", HasCampOrHotel = false, StatusId = 2 },
        new() { Name = "Athabasca Regional Boreholes", Location = "Athabasca, AB", StartDate = DateTime.UtcNow.AddDays(-100), EndDate = null, Client = "Teck Resources", HasCampOrHotel = true, StatusId = 1 },
        new() { Name = "Calgary Ring Road Geotechnical", Location = "Calgary, AB", StartDate = DateTime.UtcNow.AddDays(-45), EndDate = DateTime.UtcNow.AddDays(20), Client = "Alberta Transportation", HasCampOrHotel = false, StatusId = 1 },
        new() { Name = "Whitecourt Landfill Survey", Location = "Whitecourt, AB", StartDate = DateTime.UtcNow.AddDays(-70), EndDate = null, Client = "Town of Whitecourt", HasCampOrHotel = false, StatusId = 1 },
        new() { Name = "Drayton Valley Well Re-Entry", Location = "Drayton Valley, AB", StartDate = DateTime.UtcNow.AddDays(-110), EndDate = null, Client = "Tamarack Valley Energy", HasCampOrHotel = true, StatusId = 1 },
        new() { Name = "Edson Industrial Expansion", Location = "Edson, AB", StartDate = DateTime.UtcNow.AddDays(-35), EndDate = DateTime.UtcNow.AddDays(25), Client = "Obsidian Energy", HasCampOrHotel = true, StatusId = 1 },
        new() { Name = "Cold Lake Disposal Well", Location = "Cold Lake, AB", StartDate = DateTime.UtcNow.AddDays(-90), EndDate = null, Client = "Imperial Oil", HasCampOrHotel = true, StatusId = 1 },
        new() { Name = "Brooks Wind Farm Support", Location = "Brooks, AB", StartDate = DateTime.UtcNow.AddDays(-20), EndDate = null, Client = "BluEarth Renewables", HasCampOrHotel = false, StatusId = 1 },
        new() { Name = "Hardisty Terminal Pad Survey", Location = "Hardisty, AB", StartDate = DateTime.UtcNow.AddDays(-130), EndDate = DateTime.UtcNow.AddDays(-20), Client = "Enbridge", HasCampOrHotel = true, StatusId = 2 },
        new() { Name = "Vegreville Tank Install", Location = "Vegreville, AB", StartDate = DateTime.UtcNow.AddDays(-60), EndDate = DateTime.UtcNow.AddDays(5), Client = "AltaGas", HasCampOrHotel = false, StatusId = 1 },
        new() { Name = "Bonnyville Site Reclamation", Location = "Bonnyville, AB", StartDate = DateTime.UtcNow.AddDays(-150), EndDate = null, Client = "Orphan Well Association", HasCampOrHotel = false, StatusId = 1 }
    };

                context.Projects.AddRange(projects);
            }

            if (!await context.ProjectGroups.AnyAsync())
            {
                var groups = new List<ProjectGroup>
    {
        new() { ProjectId = 1, GroupName = "Drill Crew A" },
        new() { ProjectId = 1, GroupName = "Drill Crew B" },
        new() { ProjectId = 1, GroupName = "Support Team" },
        new() { ProjectId = 1, GroupName = "Safety Inspectors" }
    };

                context.ProjectGroups.AddRange(groups);

            }
            await context.SaveChangesAsync();

            if (!await context.Participants.AnyAsync())
            {
                var users = await context.Users.Take(8).ToListAsync();
                var groups = await context.ProjectGroups.Where(g => g.ProjectId == 1).ToListAsync();

                var participants = new List<Participant>();

                int userIndex = 0;
                foreach (var group in groups)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        if (userIndex >= users.Count) break;

                        participants.Add(new Participant
                        {
                            ProjectId = 1,
                            UserId = users[userIndex].Id,
                            GroupId = group.Id,
                            StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-10)),
                            EndDate = null,
                            DailyRate = 375m,
                            MeterRate = 0.90m,
                            DateAdded = DateTime.UtcNow
                        });

                        userIndex++;
                    }
                }

                context.Participants.AddRange(participants);

            }
            await context.SaveChangesAsync();
            if (!await context.ParticipantEquipments.AnyAsync())
            {
                var projectId = 1;
                var equipments = await context.Equipments
                    .Where(e => e.RegistrationNumber.StartsWith("123"))
                    .OrderBy(e => e.Id)
                    .Take(4)
                    .ToListAsync();

                var groups = await context.ProjectGroups
                    .Where(g => g.ProjectId == projectId)
                    .OrderBy(g => g.Id)
                    .ToListAsync();

                var participantEquipments = new List<ParticipantEquipment>();

                for (int i = 0; i < groups.Count && i < equipments.Count; i++)
                {
                    var group = groups[i];
                    var participant = context.Participants
                        .FirstOrDefault(p => p.GroupId == group.Id);

                    if (participant != null)
                    {
                        participantEquipments.Add(new ParticipantEquipment
                        {
                            ProjectId = projectId,
                            ParticipantId = participant.Id,
                            EquipmentId = equipments[i].Id,
                            StartDate = DateOnly.FromDateTime(DateTime.Today),
                            EndDate = null
                        });
                    }
                }

                context.ParticipantEquipments.AddRange(participantEquipments);

            }

            await context.SaveChangesAsync();
        }
    }
}
