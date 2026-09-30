using Api.Data;
using Api.Models.Entities;
using Api.Repositories;
using Api.Repositories.Interfaces;
using Api.Services.Implementations;
using Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Register the AppDbContext with PostgreSQL
builder.Services.AddDbContext<Api.Data.AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Unit of Work (solo SaveChanges)
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register Repositories
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<ProductoRepository>();
builder.Services.AddScoped<CategoriaRepository>();
builder.Services.AddScoped<ClienteRepository>();
builder.Services.AddScoped<ProveedorRepository>();
builder.Services.AddScoped<VentaRepository>();
builder.Services.AddScoped<CompraRepository>();
builder.Services.AddScoped<DetalleVentaRepository>();
builder.Services.AddScoped<DetalleCompraRepository>();
builder.Services.AddScoped<CajaRepository>();
builder.Services.AddScoped<MovimientoCajaRepository>();

// Register Services

// Add OpenAPI/Swagger generation (no auth for now)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pos API V1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();