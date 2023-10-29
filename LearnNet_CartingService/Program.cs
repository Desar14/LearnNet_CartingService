
using Asp.Versioning;
using FluentValidation;
using LearnNet_CartingService.Core.DTO;
using LearnNet_CartingService.Core.Interfaces;
using LearnNet_CartingService.Core.Validators;
using LearnNet_CartingService.Domain.Entities;
using LearnNet_CartingService.Domain.Services;
using LearnNet_CartingService.Domain.Validators;
using LearnNet_CartingService.Infrastructure.Data;
using LearnNet_CartingService.Infrastructure.Data.DataAccess;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

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
            builder.Services.AddApiVersioning(setup =>
            {
                setup.ReportApiVersions = true;
            })
            .AddApiExplorer(
                    options =>
                    {
                        // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                        // note: the specified format code will format the version as "'v'major[.minor][-status]"
                        options.GroupNameFormat = "'v'VVV";

                        // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                        // can also be used to control the format of the API version in route templates
                        options.SubstituteApiVersionInUrl = true;
                    }); ;
            builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            builder.Services.AddSwaggerGen(options =>
            {
                // add a custom operation filter which sets default values
                options.OperationFilter<SwaggerDefaultValues>();

                var fileName = typeof(Program).Assembly.GetName().Name + ".xml";
                var filePath = Path.Combine(AppContext.BaseDirectory, fileName);

                // integrate xml comments
                options.IncludeXmlComments(filePath);
            });

            builder.Services.AddScoped<IValidator<CartItemDTO>, CartItemDTOValidator>();
            builder.Services.AddScoped<IValidator<CartItem>, CartItemValidator>();
            builder.Services.AddScoped<IValidator<CartEntity>, CartEntityValidator>();

            builder.Services.AddSingleton<ILiteDbContext, CartingServiceLiteDbContext>();

            builder.Services.AddScoped<ICartRepository, CartRepository>();

            builder.Services.AddScoped<ICartService, CartService>();
            

            

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    var descriptions = app.DescribeApiVersions();

                    // build a swagger endpoint for each discovered API version
                    foreach (var description in descriptions)
                    {
                        var url = $"/swagger/{description.GroupName}/swagger.json";
                        var name = description.GroupName.ToUpperInvariant();
                        options.SwaggerEndpoint(url, name);
                    }
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}