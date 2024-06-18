using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FlightsApi.Data;
using FlightsApi.Services;

namespace FlightsApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<FlightsApiContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("FlightsApiContext") ?? throw new InvalidOperationException("Connection string 'FlightsApiContext' not found.")));

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddSingleton<FlightService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
