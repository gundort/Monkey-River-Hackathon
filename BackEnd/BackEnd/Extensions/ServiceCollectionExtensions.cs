using BackEnd.Models;
using BackEnd.Repositories;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace BackEnd.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            return services;
        }

        public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });

            return services;
        }

        public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration config)
        {
            var settings = new MongoDbSettings
            {
                ConnectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING")
                                   ?? config["MongoDbSettings:ConnectionString"],
                DatabaseName = Environment.GetEnvironmentVariable("MONGO_DATABASE_NAME")
                               ?? config["MongoDbSettings:DatabaseName"]
            };

            services.AddSingleton(settings);
            services.AddSingleton<MongoDbContext>();
            services.AddSingleton<IMongoDbContext, MongoDbContext>();

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration config)
        {
            var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? config["Jwt:Key"];
            var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? config["Jwt:Issuer"];
            var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? config["Jwt:Audience"];

            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("JWT secret key not found.");
            }

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtIssuer,
                        ValidAudience = jwtAudience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                    };
                });

            services.AddAuthorization();
            return services;
        }

        public static IServiceCollection AddAppDependencies(this IServiceCollection services)
        {
            services.AddScoped<JwtService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITravelAlertService, TravelAlertService>();
            services.AddScoped<IEntityService<DiagnosticTest>, EntityService<DiagnosticTest>>();
            services.AddScoped<IEntityService<MonitoredDestination>, EntityService<MonitoredDestination>>();

            services.AddScoped<IRepository<DiagnosticTest>>(provider =>
            {
                var context = provider.GetRequiredService<MongoDbContext>();
                return new Repository<DiagnosticTest>(context.Database, "DiagnosticTests");
            });

            services.AddScoped<IRepository<MonitoredDestination>>(provider =>
            {
                var context = provider.GetRequiredService<MongoDbContext>();
                return new Repository<MonitoredDestination>(context.Database, "MonitoredDestinations");
            });

            return services;
        }
    }
}
