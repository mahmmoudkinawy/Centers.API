namespace Centers.API.Extensions;
public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentityServices(
        this IServiceCollection services)
    {
        services.AddIdentityCore<UserEntity>(opts =>
        {
            opts.Password.RequireNonAlphanumeric = false;
            opts.SignIn.RequireConfirmedAccount = false;
            opts.SignIn.RequireConfirmedPhoneNumber = false;
        })
            .AddRoles<RoleEntity>()
            .AddRoleManager<RoleManager<RoleEntity>>()
            .AddUserManager<UserManager<UserEntity>>()
            .AddSignInManager<SignInManager<UserEntity>>()
            .AddEntityFrameworkStores<CentersDbContext>();

        services.AddAuthorization();

        services.AddAuthorization();

        return services;
    }
}
