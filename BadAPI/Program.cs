using BadApi.Data;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<BadDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped(typeof(BadAPI.Data.Repositories.IRepository<>), typeof(BadAPI.Data.Repositories.Repository<>));
builder.Services.AddScoped<BadAPI.Data.Repositories.IUnitOfWork, BadAPI.Data.Repositories.UnitOfWork>();
builder.Services.AddScoped<BadApi.Services.ProductService>();
builder.Services.AddScoped<BadApi.Services.CategoryService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BadDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
