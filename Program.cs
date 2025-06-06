using Cwiczenie12.Models;
using Cwiczenie12.Services;
using Microsoft.EntityFrameworkCore;

namespace Cwiczenie12;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();
        builder.Services.AddControllers();
        builder.Services.AddScoped<IDbService, DbService>();

        builder.Services.AddDbContext<TripsDbContext>(opt =>
        {
            var connectionString = builder.Configuration.GetConnectionString("Default");
            opt.UseSqlServer(connectionString);
        });

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();
        
        app.MapControllers();
        
        app.Run();
    }
}