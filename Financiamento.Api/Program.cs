using System;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Financiamento.Infrastructure.Data;
using Financiamento.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Financiamento.Application.Services;
using Financiamento.Infrastructure.Repositories;
using Financiamento.Infrastructure.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// configure listening URLs from environment (docker-compose will set ASPNETCORE_HTTP_PORTS or ASPNETCORE_URLS)
var httpPorts = builder.Configuration["ASPNETCORE_HTTP_PORTS"] ?? Environment.GetEnvironmentVariable("ASPNETCORE_HTTP_PORTS");
var aspnetUrls = builder.Configuration["ASPNETCORE_URLS"] ?? Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
if (!string.IsNullOrWhiteSpace(aspnetUrls))
{
    var urls = aspnetUrls.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).Select(u => u.Trim()).ToArray();
    builder.WebHost.UseUrls(urls);
}
else if (!string.IsNullOrWhiteSpace(httpPorts))
{
    var urls = httpPorts.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
        .Select(p => p.Trim())
        .Where(p => p.Length > 0)
        .Select(p => p.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? p : $"http://0.0.0.0:{p}")
        .ToArray();
    if (urls.Length > 0) builder.WebHost.UseUrls(urls);
}
else
{
    // default to listen on all interfaces port 80 inside container
    builder.WebHost.UseUrls("http://0.0.0.0:80");
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// configure JWT
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = Encoding.UTF8.GetBytes(jwtSection["Key"] ?? string.Empty);
var jwtIssuer = jwtSection["Issuer"];
var jwtAudience = jwtSection["Audience"];

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
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(jwtKey)
        };
    });

builder.Services.AddAuthorization();

// Swagger - add Bearer support
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// allow CORS so Swagger UI reachable from host
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
var connection = builder.Configuration.GetConnectionString("DB_Volkswagen");

builder.Services.AddDbContext<FinanciamentoDbContext>(opt => opt.UseNpgsql(connection, o => o.MigrationsHistoryTable("__EFMigrationsHistory", "volkswagen")));

// Services
builder.Services.AddScoped<IAuthenticationServices, AuthenticationServices>();
builder.Services.AddScoped<IContratosServices, ContratosServices>();
builder.Services.AddScoped<IPagamentosServices, PagamentosServices>();
builder.Services.AddScoped<IClientesServices, ClientesServices>();

// Repositories
builder.Services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
builder.Services.AddScoped<IContratosRepository, ContratosRepository>();
builder.Services.AddScoped<IPagamentosRepository, PagamentosRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(options => {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Financiamento API V1");
        options.RoutePrefix = string.Empty;   
    });
}

app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FinanciamentoDbContext>();
    db.Database.Migrate();
}

app.Run();
