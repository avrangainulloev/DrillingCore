using DrillingCore.Application.Interfaces;

using DrillingCore.Infrastructure.Persistence;
using DrillingCore.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.SpaServices;
using DrillingCore.Application.Projects.Commands.Handlers;
using Microsoft.Extensions.DependencyInjection;
using DrillingCore.Infrastructure.Services;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


// Настройка строки подключения к PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DrillingCoreDbContext>(options =>
    options.UseNpgsql(connectionString));


// Добавляем аутентификацию с JWT Bearer
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Настройка для извлечения токена из cookie
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Если токен хранится в куки с именем "AuthToken"
            var token = context.Request.Cookies["AuthToken"];
            if (!string.IsNullOrEmpty(token))
            {
                context.Token = token;
            }
            return Task.CompletedTask;
        }
    };

    // Параметры валидации токена (укажите свои значения)
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "DrillingCoreIssuer",
        ValidAudience = "DrillingCoreAudience",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Your_Secret_Key_Here_That_Is_At_Least_32_Characters_Long"))
    };
});

builder.Services.AddAuthorization();



// Регистрация зависимостей
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IParticipantRepository, ParticipantRepository>();
builder.Services.AddScoped<IProjectGroupRepository, ProjectGroupRepository>();
 
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
// Регистрируем MediatR (указываем сборку с обработчиками команд) 
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CreateProjectCommandHandler).Assembly);
});


builder.Services.AddControllers();

// Настройка JWT

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Включаем статические файлы (см. шаг 7)
builder.Services.AddSpaStaticFiles(configuration => {
    configuration.RootPath = "wwwroot";
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.AddHttpContextAccessor();


var app = builder.Build();


// Включаем раздачу статических файлов
app.UseDefaultFiles();   // Автоматически ищет index.html, default.html и т.д.
app.UseStaticFiles();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseCors("AllowAll");

app.Run();
