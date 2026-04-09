using Financiamento.Infrastructure.Data;
using Financiamento.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Financiamento.Domain.Repositories;
using Financiamento.Application.Services;
using Financiamento.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connection = builder.Configuration.GetConnectionString("DB_Volkswagen");

builder.Services.AddDbContext<FinanciamentoDbContext>(opt => opt.UseNpgsql(connection, o => o.MigrationsHistoryTable("__EFMigrationsHistory", "volkswagen")));


// Services
builder.Services.AddScoped<IContratosServices, ContratosServices>();
builder.Services.AddScoped<IPagamentosServices, PagamentosServices>();

// Repositories
builder.Services.AddScoped<IContratosRepository, ContratosRepository>();
builder.Services.AddScoped<IPagamentosRepository, PagamentosRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();
