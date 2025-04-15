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
using System.Reflection;
using DrillingCore.Infrastructure;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Настройка строки подключения к PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DrillingCoreDbContext>(options =>
    options.UseNpgsql(connectionString));

// JWT-аутентификация
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var token = context.Request.Cookies["AuthToken"];
            if (!string.IsNullOrEmpty(token))
            {
                context.Token = token;
            }
            return Task.CompletedTask;
        }
    };

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

builder.Services.AddInfrastructure();
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CreateProjectCommandHandler).Assembly);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "wwwroot";
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowDev",
        policy => policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
    );
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DrillingCoreDbContext>();

    // 1. Применить миграции, если база не существует — она будет создана
    await context.Database.MigrateAsync();

    // 2. Теперь можно выполнять сидирование
    await SeedData.SeedAsync(context);
}

// Важно: сначала UseDefaultFiles и UseStaticFiles
app.UseDefaultFiles();
//app.UseStaticFiles();

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        var path = ctx.Context.Request.Path.Value;
        if (path != null && (path.StartsWith("/photos") || path.StartsWith("/signatures")))
        {
            ctx.Context.Response.Headers["Access-Control-Allow-Origin"] = "http://localhost:5173";
            ctx.Context.Response.Headers["Access-Control-Allow-Credentials"] = "true";
        }
    }
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowDev");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// SPA fallback
app.MapFallbackToFile("index.html");

app.Run();
