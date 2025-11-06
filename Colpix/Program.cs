using Colpix.Data.Models;
using Colpix.Data.Repositories;
using Colpix.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Bind JwtSettings from configuration (appsettings.json) or use defaults in JwtSettings class
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

// Add EF Core In-Memory
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("ColpixDb"));

builder.Services.AddControllers();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
// Register token service
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
// Configure authentication with JWT Bearer
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>() ?? new JwtSettings();
var keyBytes = Encoding.UTF8.GetBytes(jwtSettings.Key);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ClockSkew = TimeSpan.Zero // optional: no clock skew for precise expiry
    };
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Colpix API", Version = "v1", Description = @"API de gestión de empleados.  
        <br><br>
        **Autenticación:**  
        Todos los endpoints marcados con `[Authorize]` requieren un token JWT válido.  
        <br><br>
        Para probarlos en Swagger, presioná el botón **'Authorize'** arriba a la derecha  
        y pegá el token en formato:
        <br><br>
        `eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Ingrese el token JWT",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", securityScheme);
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, new string[] { } }
    });
});

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!db.Users.Any())
    {
        var hasher = new PasswordHasher<User>();
        var demo = new User
        {
            Username = "colpix",
            LastUpdate = DateTime.UtcNow
        };
        demo.PasswordHash = hasher.HashPassword(demo, "colpix");
        db.Users.Add(demo);
        db.SaveChanges();
    }

    db.Employees.AddRange(new Employee { Name = "Empleado_1", Email = "Empleado_1@colpix.com", Supervisor_id = 0 },
                new Employee { Name = "Empleado_2", Email = "Empleado_2@colpix.com", Supervisor_id = 1 },
                new Employee { Name = "Empleado_3", Email = "Empleado_3@colpix.com", Supervisor_id = 1 },
                new Employee { Name = "Empleado_4", Email = "Empleado_4@colpix.com", Supervisor_id = 1 },
                new Employee { Name = "Empleado_5", Email = "Empleado_5@colpix.com", Supervisor_id = 2 },
                new Employee { Name = "Empleado_6", Email = "Empleado_6@colpix.com", Supervisor_id = 3 },
                new Employee { Name = "Empleado_7", Email = "Empleado_7@colpix.com", Supervisor_id = 3 },
                new Employee { Name = "Empleado_8", Email = "Empleado_8@colpix.com", Supervisor_id = 3 },
                new Employee { Name = "Empleado_9", Email = "Empleado_9@colpix.com", Supervisor_id = 3 });
    db.SaveChanges();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Colpix API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
