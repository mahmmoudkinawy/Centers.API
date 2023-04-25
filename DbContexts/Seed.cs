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

        var centerAdminUser = new UserEntity
        {
            Id = Guid.NewGuid(),
            FirstName = "Admin",
            LastName = "Center",
            Email = "admincenter@test.com",
            UserName = "admincenter@test.com",
            Gender = "Female",
            PhoneNumber = "01208534241",
            PhoneNumberConfirmed = true
        };

        await userManager.CreateAsync(centerAdminUser, "Pa$$w0rd");
        await userManager.AddToRoleAsync(centerAdminUser, Constants.Roles.CenterAdmin);

        var teacherUser = new UserEntity
        {
            Id = Guid.NewGuid(),
            FirstName = "Teacher",
            LastName = "Yasso",
            Email = "teacher@test.com",
            UserName = "teacher@test.com",
            Gender = "Male",
            PhoneNumber = "01204595826",
            PhoneNumberConfirmed = true
        };

        await userManager.CreateAsync(teacherUser, "Pa$$w0rd");
        await userManager.AddToRoleAsync(teacherUser, Constants.Roles.Teacher);

        var reviewerUser = new UserEntity
        {
            Id = Guid.NewGuid(),
            FirstName = "Reviewer",
            LastName = "Mo",
            Email = "reviewer@test.com",
            UserName = "reviewer@test.com",
            Gender = "Female",
            PhoneNumber = "01104595826",
            PhoneNumberConfirmed = true
        };

        await userManager.CreateAsync(reviewerUser, "Pa$$w0rd");
        await userManager.AddToRoleAsync(reviewerUser, Constants.Roles.Reviewer);

        // Seeding some fake users for testing.
        var fakeStudents = new Faker<UserEntity>()
            .RuleFor(u => u.Id, f => f.Random.Guid())
            .RuleFor(u => u.FirstName, f => f.Person.FirstName)
            .RuleFor(u => u.LastName, f => f.Person.LastName)
            .RuleFor(u => u.Email, f => f.Person.Email)
            .RuleFor(u => u.UserName, f => f.Person.UserName)
            .RuleFor(u => u.PhoneNumber, f => f.Person.Phone)
            .RuleFor(u => u.PhoneNumberConfirmed, f => true)
            .RuleFor(u => u.Gender, f => (new[] { "Female", "Male" })[new Random().Next(2)]);

        foreach (var student in fakeStudents.GenerateBetween(100, 150))
        {
            await userManager.CreateAsync(student, "Pa$$w0rd");
            await userManager.AddToRoleAsync(student, Constants.Roles.Student);
        }

    }

}
