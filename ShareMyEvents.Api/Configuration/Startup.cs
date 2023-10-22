﻿using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ShareMyEvents.Api.Data;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ShareMyEvents.Domain.Interfaces;
using ShareMyEvents.Api.Services;
using NSwag;
using NSwag.Generation.Processors.Security;

namespace ShareMyEvents.Api.Configuration;
internal class Startup
{
    private static WebApplicationBuilder _builder;

    public static void Initialize (string[] args)
    {
        if (_builder == null)
        {
            _builder = WebApplication.CreateBuilder(args);
        }

        ConfigureServices ();
        Configure();
    }

    private static void ConfigureServices()
    {
        _builder?.Host.ConfigureServices((context, services) =>
        {
            // DbContext
            ConfigureDbContextService(services);

            services.AddControllers();

            // Swagger
            ConfigureSwaggerService(services);

            // Authentication with JWT
            ConfigureAuthenticationService(services, context);

            RegisterDomainServices(services);
        });
    }

    private static void Configure ()
    {
        var app = _builder.Build();

        // Configure the HTTP request pipeline.
        if(app.Environment.IsDevelopment())
        {
            app.UseOpenApi();
            app.UseSwaggerUi3();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }

    private static void ConfigureDbContextService(IServiceCollection services)
    {
        services.AddDbContext<ShareMyEventsApiContext>(dbContextOptions =>
        {
            IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

            var connectionString = configuration.GetConnectionString("ShareMyEventsApiContext") ?? throw new InvalidOperationException("Connection string 'ShareMyEventsApiContext' not found.");
            dbContextOptions.UseSqlServer(connectionString);
        });
    }

    private static void ConfigureAuthenticationService(IServiceCollection services, HostBuilderContext context)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            //options.SaveToken = true;
            options.TokenValidationParameters = new()
            {
                ValidateIssuerSigningKey = true,
                ValidateAudience = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(context.Configuration["Jwt:Secret"])),
                ValidIssuer = context.Configuration["Jwt:ValidIssuer"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });
    }

    private static void ConfigureSwaggerService(IServiceCollection services)
    {
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();

        services.AddOpenApiDocument(options =>
        {
            options.PostProcess = document =>
            {
                document.Info = new OpenApiInfo
                {
                    Version = "v1",
                    Title = "ShareMyEvents Api"
                };
            };

            options.OperationProcessors.Add(new OperationSecurityScopeProcessor("JWT Token"));

            options.AddSecurity("JWT Token", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = OpenApiSecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = OpenApiSecurityApiKeyLocation.Header,
                Description = "Enter Bearer [space] and then your valid token in the input text below"
            });
        });
    }

    private static void RegisterDomainServices(IServiceCollection services)
    {
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IEventService, EventService>();
        services.AddScoped<IParticipationService, ParticipationService>();
        services.AddScoped<CancellationTokenSource, CancellationTokenSource>();
    }
}