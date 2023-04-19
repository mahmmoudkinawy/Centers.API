namespace Centers.API.DbContexts;
public sealed class CentersDbContext : IdentityDbContext<UserEntity, RoleEntity, Guid,
    IdentityUserClaim<Guid>, IdentityUserRole<Guid>, IdentityUserLogin<Guid>,
    IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
{
    public CentersDbContext(DbContextOptions<CentersDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UserEntity>()
            .HasOne(d => d.Disability)
            .WithOne(u => u.Owner)
            .HasForeignKey<DisabilityEntity>(d => d.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<DisabilityEntity>()
            .HasOne(u => u.Owner)
            .WithOne(d => d.Disability)
            .HasForeignKey<UserEntity>(u => u.DisabilityId)
            .OnDelete(DeleteBehavior.Restrict);
    }

}
