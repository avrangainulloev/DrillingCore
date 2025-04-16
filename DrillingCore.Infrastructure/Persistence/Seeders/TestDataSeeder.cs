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
                var users = new List<User>();

                for (int i = 1; i <= 40; i++)
                {
                    users.Add(new User
                    {
                        FullName = $"Test User {i}",
                        Username = $"testuser{i}",
                        PasswordHash = "hashedpassword", // или используйте хэширование, если нужно
                        RoleId = 2,
                        Email = $"testuser{i}@example.com",
                        Mobile = $"555-010{i:D2}", // 555-01001, 555-01002, ...
                        IsActive = true
                    });
                }

                context.Users.AddRange(users);

            }

            if (!await context.Projects.AnyAsync())
            {
                var projects = new List<Project>();

                for (int i = 1; i <= 20; i++)
                {
                    projects.Add(new Project
                    {
                        Name = $"Project {i}",
                        Location = $"Location {i}",
                        StartDate = DateTime.UtcNow.AddDays(-i * 10), // старт каждый на 10 дней раньше
                        EndDate = i % 3 == 0 ? DateTime.UtcNow.AddDays(i * 5) : null, // некоторые без даты окончания
                        Client = $"Client {i}",
                        HasCampOrHotel = i % 2 == 0, // чередование true/false
                        StatusId = i % 2 == 0 ? 1 : 2 // например: 1 = Open, 2 = Completed
                    });
                }

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
