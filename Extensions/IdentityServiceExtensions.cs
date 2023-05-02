namespace Centers.API.Extensions;
public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentityServices(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddIdentityCore<UserEntity>(opts =>
        {
            opts.Password.RequireNonAlphanumeric = false;
            opts.User.RequireUniqueEmail = true;
            opts.SignIn.RequireConfirmedAccount = false;
            opts.SignIn.RequireConfirmedPhoneNumber = true;
        })
            .AddRoles<RoleEntity>()
            .AddRoleManager<RoleManager<RoleEntity>>()
            .AddSignInManager<SignInManager<UserEntity>>()
            .AddEntityFrameworkStores<CentersDbContext>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(opts =>
                    {
                        opts.TokenValidationParameters = new()
                        {
                            ValidateAudience = false,
                            ValidateIssuer = false,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(config[Constants.TokenKey]!)),
                            ClockSkew = TimeSpan.Zero
                        };
                    });

        services.AddAuthorization(builder =>
        {
            builder.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            builder.AddPolicy(Constants.Policies.MustBeSuperAdmin, opts =>
            {
                opts.RequireClaim(ClaimTypes.Role, Constants.Roles.SuperAdmin);
            });

            builder.AddPolicy(Constants.Policies.MustBeTeacher, opts =>
            {
                opts.RequireClaim(ClaimTypes.Role, Constants.Roles.Teacher);
            });

        });

        return services;
    }
}
