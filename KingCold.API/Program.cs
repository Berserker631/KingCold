using KingCold.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<KingColdDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("KingColdConnection")
    ));

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

//app.UseHttpsRedirection();

//if (app.Environment.IsDevelopment())
//{
//    app.MapOpenApi();
//}

app.UseCors("AllowAll");

app.MapControllers();

app.Run();