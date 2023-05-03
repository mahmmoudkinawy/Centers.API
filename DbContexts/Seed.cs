namespace Centers.API.DbContexts;
public static class Seed
{
    public static async Task SeedRolesAsync(
        RoleManager<RoleEntity> roleManager)
    {
        ArgumentNullException.ThrowIfNull(nameof(roleManager));

        if (await roleManager.Roles.AnyAsync())
        {
            return;
        }

        var roles = new List<RoleEntity>()
        {
            new RoleEntity
            {
                Id = Guid.NewGuid(),
                Name = Constants.Roles.Student
            },
            new RoleEntity
            {
                Id = Guid.NewGuid(),
                Name = Constants.Roles.Teacher
            },
            new RoleEntity
            {
                Id = Guid.NewGuid(),
                Name = Constants.Roles.Reviewer
            },
            new RoleEntity
            {
                Id = Guid.NewGuid(),
                Name = Constants.Roles.CenterAdmin
            },
            new RoleEntity
            {
                Id = Guid.NewGuid(),
                Name = Constants.Roles.SuperAdmin
            }
        };

        foreach (var role in roles)
        {
            await roleManager.CreateAsync(role);
        }
    }

    public static async Task SeedUsersAsync(UserManager<UserEntity> userManager)
    {
        ArgumentNullException.ThrowIfNull(nameof(userManager));

        if (await userManager.Users.AnyAsync())
        {
            return;
        }

        var students = new List<UserEntity>()
        {
            new UserEntity
            {
                Id = Guid.NewGuid(),
                FirstName = "Bob",
                LastName = "Yasso",
                Email = "bob@test.com",
                UserName = "bob@test.com",
                Gender = "Male",
                PhoneNumber = "01208534246",
                PhoneNumberConfirmed = true
            },
            new UserEntity
            {
                Id = Guid.NewGuid(),
                FirstName = "Lisa",
                LastName = "Komal",
                Email = "lisa@test.com",
                UserName = "lisa@test.com",
                Gender = "Female",
                PhoneNumber = "01204595826",
                PhoneNumberConfirmed = true
            }
        };

        foreach (var student in students)
        {
            await userManager.CreateAsync(student, "Pa$$w0rd");
            await userManager.AddToRoleAsync(student, Constants.Roles.Student);
        }

        var superAdminUser = new UserEntity
        {
            Id = Guid.NewGuid(),
            FirstName = "Super",
            LastName = "Admin",
            Email = "superadmin@test.com",
            UserName = "superadmin@test.com",
            Gender = "Male",
            PhoneNumber = "01208536213",
            PhoneNumberConfirmed = true
        };

        await userManager.CreateAsync(superAdminUser, "Pa$$w0rd");
        await userManager.AddToRolesAsync(superAdminUser, new[]
        {
            // Will be modified later on.
            Constants.Roles.SuperAdmin,
            Constants.Roles.CenterAdmin,
            Constants.Roles.Reviewer,
            Constants.Roles.Teacher,
            Constants.Roles.Student
        });

        var centerAdminUsers = new List<UserEntity>
        {
            new UserEntity
            {
                Id = Guid.NewGuid(),
                FirstName = "Admin",
                LastName = "Center 1",
                NationalId = "15-536551-551",
                Email = "admincenter1@test.com",
                UserName = "admincenter1@test.com",
                Gender = "Female",
                PhoneNumber = "01208534241",
                PhoneNumberConfirmed = true
            },
            new UserEntity
            {
                Id = Guid.NewGuid(),
                FirstName = "Admin",
                LastName = "Center 2",
                NationalId = "15-536551-552",
                Email = "admincenter2@test.com",
                UserName = "admincenter2@test.com",
                Gender = "Male",
                PhoneNumber = "01208534245",
                PhoneNumberConfirmed = true
            },
            new UserEntity
            {
                Id = Guid.NewGuid(),
                FirstName = "Admin",
                LastName = "Center 3",
                NationalId = "15-536551-552",
                Email = "admincenter3@test.com",
                UserName = "admincenter3@test.com",
                Gender = "Female",
                PhoneNumber = "01272975803",
                PhoneNumberConfirmed = true
            },
        };

        foreach (var centerAdminUser in centerAdminUsers)
        {
            await userManager.CreateAsync(centerAdminUser, "Pa$$w0rd");
            await userManager.AddToRoleAsync(centerAdminUser, Constants.Roles.CenterAdmin);
        }

        var teacherUsers = new List<UserEntity>()
        {
            new UserEntity
            {
                Id = Guid.NewGuid(),
                FirstName = "Teacher",
                LastName = "Yasso 1",
                NationalId = "15-53632-65",
                Email = "teacher1@test.com",
                UserName = "teacher1@test.com",
                Gender = "Female",
                PhoneNumber = "01204595822",
                PhoneNumberConfirmed = true
            },
            new UserEntity
            {
                Id = Guid.NewGuid(),
                FirstName = "Teacher",
                LastName = "Soma 2",
                NationalId = "15-56632-65",
                Email = "teacher2@test.com",
                UserName = "teacher2@test.com",
                Gender = "Male",
                PhoneNumber = "01271128534",
                PhoneNumberConfirmed = true
            },
            new UserEntity
            {
                Id = Guid.NewGuid(),
                FirstName = "Teacher",
                LastName = "Kino 3",
                NationalId = "15-53362-55",
                Email = "teacher3@test.com",
                UserName = "teacher3@test.com",
                Gender = "Male",
                PhoneNumber = "01271128542",
                PhoneNumberConfirmed = true
            }
        };

        foreach (var teacherUser in teacherUsers)
        {
            await userManager.CreateAsync(teacherUser, "Pa$$w0rd");
            await userManager.AddToRoleAsync(teacherUser, Constants.Roles.Teacher);
        }

        var reviewerUser = new UserEntity
        {
            Id = Guid.NewGuid(),
            FirstName = "Reviewer",
            LastName = "Mo",
            NationalId = "15-12351-121",
            Email = "reviewer@test.com",
            UserName = "reviewer@test.com",
            Gender = "Female",
            PhoneNumber = "01104595826",
            PhoneNumberConfirmed = true
        };

        await userManager.CreateAsync(reviewerUser, "Pa$$w0rd");
        await userManager.AddToRoleAsync(reviewerUser, Constants.Roles.Reviewer);

        // Seeding some fake users for testing.
        var fakeStudents = new Faker<UserEntity>("ar")
            .RuleFor(u => u.Id, f => Guid.NewGuid())
            .RuleFor(u => u.FirstName, f => f.Person.FirstName)
            .RuleFor(u => u.LastName, f => f.Person.LastName)
            .RuleFor(u => u.Email, f => f.Person.Email)
            .RuleFor(u => u.UserName, f => f.Person.UserName)
            .RuleFor(u => u.PhoneNumber, f => f.Person.Phone)
            .RuleFor(u => u.PhoneNumberConfirmed, f => true)
            .RuleFor(u => u.NationalId, f => f.Person.Random.AlphaNumeric(15))
            .RuleFor(u => u.Gender, f => (new[] { "Female", "Male" })[new Random().Next(2)])
            .Generate(70);

        foreach (var student in fakeStudents)
        {
            await userManager.CreateAsync(student, "Pa$$w0rd");
            await userManager.AddToRoleAsync(student, Constants.Roles.Student);
        }

    }

    public static async Task SeedSubjectsAndCenters(CentersDbContext context)
    {
        ArgumentNullException.ThrowIfNull(nameof(context));

        if (await context.Subjects.AnyAsync())
        {
            return;
        }

        var fakeSubjects = new Faker<SubjectEntity>("ar")
            .RuleFor(s => s.Id, f => Guid.NewGuid())
            .RuleFor(s => s.Name, f => f.Hacker.Verb())
            .RuleFor(s => s.Description, f => f.Lorem.Paragraph(4))
            .Generate(150);

        context.Subjects.AddRange(fakeSubjects);
        await context.SaveChangesAsync();

        var fakeCenters = new Faker<CenterEntity>("ar")
            .RuleFor(s => s.Id, f => Guid.NewGuid())
            .RuleFor(s => s.Name, f => f.Lorem.Word())
            .RuleFor(s => s.Capacity, f => f.Random.Int(5, 10000))
            .RuleFor(s => s.Description, f => f.Lorem.Paragraph(3))
            .RuleFor(s => s.ClosingDate, f => f.Date.Between(DateTime.UtcNow, DateTime.UtcNow.AddYears(1)))
            .RuleFor(s => s.OpeningDate, f => f.Date.Between(DateTime.UtcNow.AddYears(-2), DateTime.UtcNow))
            .Generate(160);

        context.Centers.AddRange(fakeCenters);
        await context.SaveChangesAsync();
    }
}
