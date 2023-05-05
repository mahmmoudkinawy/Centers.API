namespace Centers.API.DbContexts;
public sealed class CentersDbContext : IdentityDbContext<UserEntity, RoleEntity, Guid,
    IdentityUserClaim<Guid>, IdentityUserRole<Guid>, IdentityUserLogin<Guid>,
    IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
{
    public CentersDbContext(DbContextOptions<CentersDbContext> options) : base(options) { }

    public DbSet<DisabilityEntity> Disabilities { get; set; }
    public DbSet<OtpEntity> Otps { get; set; }
    public DbSet<SubjectEntity> Subjects { get; set; }
    public DbSet<CenterEntity> Centers { get; set; }
    public DbSet<ImageEntity> Images { get; set; }
    public DbSet<ShiftEntity> Shifts { get; set; }
    public DbSet<ShiftSubjectEntity> ShiftSubjects { get; set; }
    public DbSet<ChoiceEntity> Choices { get; set; }
    public DbSet<QuestionEntity> Questions { get; set; }
    public DbSet<AnswerEntity> Answers { get; set; }

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

        builder.Entity<OtpEntity>()
            .HasIndex(o => o.PhoneNumber);

        builder.Entity<UserEntity>()
            .HasMany(i => i.Images)
            .WithOne(u => u.User)
            .HasForeignKey(ui => ui.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<UserEntity>()
            .Property(u => u.IsActive)
            .HasDefaultValue(true);

        builder.Entity<ShiftEntity>()
            .HasOne(c => c.Center)
            .WithMany(s => s.Shifts)
            .HasForeignKey(k => k.CenterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ShiftEntity>()
            .HasOne(a => a.Admin)
            .WithMany()
            .HasForeignKey(k => k.AdminId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ShiftSubjectEntity>()
            .HasKey(ss => new { ss.ShiftId, ss.SubjectId });

        builder.Entity<ShiftSubjectEntity>()
            .HasOne(ss => ss.Shift)
            .WithMany(s => s.ShiftSubjects)
            .HasForeignKey(ss => ss.ShiftId);

        builder.Entity<ShiftSubjectEntity>()
            .HasOne(ss => ss.Subject)
            .WithMany()
            .HasForeignKey(ss => ss.SubjectId);

        builder.Entity<ChoiceEntity>()
            .HasOne(q => q.Question)
            .WithMany(c => c.Choices)
            .HasForeignKey(k => k.QuestionId);

        builder.Entity<UserEntity>()
            .HasMany(u => u.Questions)
            .WithOne(q => q.Owner)
            .HasForeignKey(k => k.OwnerId);

        builder.Entity<QuestionEntity>()
            .HasOne(q => q.Owner)
            .WithMany(u => u.Questions)
            .HasForeignKey(q => q.OwnerId);

        builder.Entity<QuestionEntity>()
            .HasOne(q => q.Answer)
            .WithOne(q => q.Question)
            .HasForeignKey<AnswerEntity>(a => a.QuestionId);

        builder.Entity<AnswerEntity>()
            .HasOne(q => q.Question)
            .WithOne(q => q.Answer)
            .HasForeignKey<QuestionEntity>(a => a.AnswerId);

        builder.Entity<QuestionEntity>()
            .HasMany(i => i.Images)
            .WithOne(q => q.Question)
            .HasForeignKey(i => i.QuestionId);

        builder.ApplyUtcDateTimeConverter();
    }

}
