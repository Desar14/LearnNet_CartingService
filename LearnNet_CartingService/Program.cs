
using FluentValidation;
using LearnNet_CartingService.Core.DTO;
using LearnNet_CartingService.Core.Interfaces;
using LearnNet_CartingService.Domain.Services;
using LearnNet_CartingService.Infrastructure.Data;
using LearnNet_CartingService.Infrastructure.Data.DataAccess;

namespace LearnNet_CartingService
{
	public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.Configure<LiteDbOptions>(builder.Configuration.GetSection("LiteDbOptions"));

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddScoped<IValidator<CartItemDTO>, CartItemDTOValidator>();

            builder.Services.AddSingleton<ILiteDbContext, CartingServiceLiteDbContext>();

            builder.Services.AddScoped<ICartRepository, CartRepository>();

            builder.Services.AddScoped<ICartService, CartService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}