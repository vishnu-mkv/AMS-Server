using AMS.Authorization;
using AMS.Data;
using AMS.Data.Seeders;
using AMS.Interfaces;
using AMS.Managers;
using AMS.Managers.Auth;
using AMS.Middlewares;
using AMS.Providers;
using AMS.Utils;
using AMS.Validators;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Database connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.EnableSensitiveDataLogging();
});


builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// ----------- validators
builder.Services.AddValidatorsFromAssemblyContaining<UserValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateUserValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<AddRoleValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateRoleValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<AddScheduleValidator>();
// ------------


builder.Services.AddSingleton<ISecurePasswordHasher, PasswordHasher>();
builder.Services.AddSingleton<IFileManager, LocalFileManager>();
builder.Services.AddSingleton<IJwtProvider, JwtProvider>();

builder.Services.AddSingleton<IRoleProvider, RoleProvider>();
builder.Services.AddScoped<IRoleManager, RoleManager>();
builder.Services.AddScoped<IUserManager, UserManager>();
builder.Services.AddScoped<IScheduleManager, ScheduleManager>();
builder.Services.AddSingleton<IOrganizationProvider, OrganizationProvider>();
builder.Services.AddScoped<IAuthManager, AuthManager>();


builder.Services.AddAuthorization();
builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

builder.Services.AddControllers(options =>
    options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer())))
   .AddJsonOptions(options =>
   {
       options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
   });

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwt = builder.Configuration.GetSection("Jwt").Get<Jwt>();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key))
        };
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setup =>
{
    // Include 'SecurityScheme' to use JWT Authentication
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "JWT Authentication",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

    setup.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});

const string MyAllowSpecificOrigins = "Allowed Origins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
                      });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();


// seed database
var permissionsSeeder = new PermissionSeeder(app.Services);
permissionsSeeder.Seed();

var adminSeeder = new AdminUserSeeder(app.Services);
adminSeeder.Seed();

app.Run();
