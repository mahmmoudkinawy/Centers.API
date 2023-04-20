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
            opts.SignIn.RequireConfirmedPhoneNumber = true;
        })
            .AddRoles<RoleEntity>()
            .AddRoleManager<RoleManager<RoleEntity>>()
            .AddSignInManager<SignInManager<UserEntity>>()
            .AddEntityFrameworkStores<CentersDbContext>();

        services.AddAuthentication();

        services.AddAuthorization(builder =>
        {
            builder.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        return services;
    }
}
