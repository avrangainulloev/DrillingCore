using DrillingCore.Application.Interfaces;
using DrillingCore.Infrastructure.Persistence;
using DrillingCore.Infrastructure.Repositories;
using DrillingCore.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrillingCore.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IFileStorageService, FileStorageService>();
            // Регистрация зависимостей
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IParticipantRepository, ParticipantRepository>();
            services.AddScoped<IProjectGroupRepository, ProjectGroupRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IFormRepository, FormRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IEquipmentRepository, EquipmentRepository>();
            services.AddScoped<IParticipantEquipmentRepository, ParticipantEquipmentRepository>();
            services.AddScoped<IEquipmentTypeRepository, EquipmentTypeRepository>();
            services.AddScoped<IChecklistRepository, ChecklistItemRepository>();
            services.AddScoped<IFlhaRepository, FlhaRepository>();

            return services;
        }
    }
}
