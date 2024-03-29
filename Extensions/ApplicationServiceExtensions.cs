﻿namespace Centers.API.Extensions;
public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddScoped<ITokenService, TokenService>();

        services.AddScoped<IPhotoService, PhotoService>();

        services.AddTransient<ISmsService, SmsService>();

        services.AddTransient<IEmailSender, EmailSender>();

        services.Configure<TwilioSettings>(config.GetSection("TwilioSettings"));

        services.AddScoped<IOtpService, OtpService>();

        services.AddMediatR(_ => _.RegisterServicesFromAssemblyContaining<Program>());

        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        services.AddControllers()
            .AddFluentValidation(_ => _.RegisterValidatorsFromAssemblyContaining<Program>());

        services.AddDbContext<CentersDbContext>(opts
                => opts.UseSqlServer(config.GetConnectionString("DefaultConnection")));

        return services;
    }

    public static async Task ConfigureDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CentersDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<RoleEntity>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserEntity>>();
        try
        {
            await dbContext.Database.MigrateAsync();
            await Seed.SeedSubjectsAndExamDates(dbContext);
            await Seed.SeedRolesAsync(roleManager);
            await Seed.SeedUsersAsync(userManager);
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while applying migrations.");
        }
    }

}
