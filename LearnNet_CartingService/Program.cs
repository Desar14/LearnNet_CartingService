
using Asp.Versioning;
using FluentValidation;
using LearnNet_CartingService.Auth;
using LearnNet_CartingService.Core.DTO;
using LearnNet_CartingService.Core.Interfaces;
using LearnNet_CartingService.Core.Validators;
using LearnNet_CartingService.Domain.Entities;
using LearnNet_CartingService.Domain.Services;
using LearnNet_CartingService.Domain.Validators;
using LearnNet_CartingService.Infrastructure.Data;
using LearnNet_CartingService.Infrastructure.Data.DataAccess;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
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

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = builder.Configuration["JWT:ValidIssuer"];

                options.TokenValidationParameters.ValidateAudience = true;
                options.Audience = builder.Configuration["JWT:ValidAudience"];

                // it's recommended to check the type header to avoid "JWT confusion" attacks
                options.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy(Policies.Read, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireAssertion(x => x.User.HasClaim(x => x.Type == "scope" && x.Value.Contains("read")));
                });

                options.AddPolicy(Policies.Create, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireAssertion(x => x.User.HasClaim(x => x.Type == "scope" && x.Value.Contains("create")));
                });

                options.AddPolicy(Policies.Update, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireAssertion(x => x.User.HasClaim(x => x.Type == "scope" && x.Value.Contains("update")));
                });

                options.AddPolicy(Policies.Delete, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireAssertion(x => x.User.HasClaim(x => x.Type == "scope" && x.Value.Contains("delete")));
                });
            });

            builder.Services.AddEndpointsApiExplorer();

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

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });

            builder.Services.AddScoped<IValidator<CartItemDTO>, CartItemDTOValidator>();
            builder.Services.AddScoped<IValidator<CartItem>, CartItemValidator>();
            builder.Services.AddScoped<IValidator<CartEntity>, CartEntityValidator>();

            builder.Services.AddSingleton<ILiteDbContext, CartingServiceLiteDbContext>();

            builder.Services.AddScoped<ICartRepository, CartRepository>();

            builder.Services.AddScoped<ICartService, CartService>();

            builder.Services.AddSingleton<
                   IAuthorizationMiddlewareResultHandler, AuthLogMiddleware>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.

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


            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}