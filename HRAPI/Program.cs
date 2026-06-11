using HRAPI.Data;
using HRAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using HRAPI.Services;
using HRAPI.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IPositionService, PositionService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<ILeaveTypeService, LeaveTypeService>();
builder.Services.AddScoped<ILeaveRequestService, LeaveRequestService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    // password policy
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// AddIdentity registers cookie handlers that hijack the default schemes.
// Re-register JWT Bearer as the default after AddIdentity.
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("JWT validated for {Email}", context.Principal?.Identity?.Name);
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError(context.Exception, "JWT authentication failed");
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogWarning("JWT challenge: {Error}, {Description}", context.Error, context.ErrorDescription);
            return Task.CompletedTask;
        }
    };
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});
// Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
var app = builder.Build();

// Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseAuthentication();   // ← before UseAuthorization
app.UseAuthorization();    // ← already assumed but not present

// Commented for local development because HTTPS port is not configured.
// app.UseHttpsRedirection();
app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

    string[] roles = { "Admin", "HRManager", "TeamLead", "Employee" };
    foreach (var role in roles)
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));

    // Admin
    var admin = await userManager.FindByEmailAsync("admin@hr.com");
    if (admin == null)
    {
        admin = new AppUser { UserName = "admin@hr.com", Email = "admin@hr.com" };
        await userManager.CreateAsync(admin, "Admin123!");
        await userManager.AddToRoleAsync(admin, "Admin");
    }

    // HR Manager
    var hr = await userManager.FindByEmailAsync("hr@hr.com");
    if (hr == null)
    {
        hr = new AppUser { UserName = "hr@hr.com", Email = "hr@hr.com", EmployeeId = null };
        await userManager.CreateAsync(hr, "Hr123!");
        await userManager.AddToRoleAsync(hr, "HRManager");
    }

    // Employee
    var emp = await userManager.FindByEmailAsync("employee@hr.com");
    if (emp == null)
    {
        emp = new AppUser { UserName = "employee@hr.com", Email = "employee@hr.com", EmployeeId = null };
        await userManager.CreateAsync(emp, "Emp123!");
        await userManager.AddToRoleAsync(emp, "Employee");
    }

    // Team Lead
    var tl = await userManager.FindByEmailAsync("teamlead@hr.com");
    if (tl == null)
    {
        tl = new AppUser { UserName = "teamlead@hr.com", Email = "teamlead@hr.com", EmployeeId = null };
        await userManager.CreateAsync(tl, "Team123!");
        await userManager.AddToRoleAsync(tl, "TeamLead");
    }
}
app.Run();
