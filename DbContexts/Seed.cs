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
                PhoneNumberConfirmed = true,
                NationalId = "546546465-5120",
                Zone = "Abu Dhabi"
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
                PhoneNumberConfirmed = true,
                NationalId = "54654-5-5120",
                Zone = "UAE"
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
            PhoneNumberConfirmed = true,
            NationalId = "3261-52-45-755",
            Zone = "Fujairah"
        };

        await userManager.CreateAsync(superAdminUser, "Pa$$w0rd");
        await userManager.AddToRolesAsync(superAdminUser, new[]
        {
            Constants.Roles.SuperAdmin
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
                PhoneNumberConfirmed = true,
                Zone = "Ajman Free Zone",
                Center = new CenterEntity
                {
                    Id = Guid.NewGuid(),
                    Capacity = 50,
                    Gender = "Both",
                    IsEnabled = true,
                    LocationUrl = "https://github.com/mahmmoud",
                    Zone = "Abu Dhabi",
                    Name = "Ibn Elhithem"
                }
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
                PhoneNumberConfirmed = true,
                Zone = "Fujairah",
                Center = new CenterEntity
                {
                    Id = Guid.NewGuid(),
                    Capacity = 55,
                    Gender = "Male",
                    IsEnabled = true,
                    LocationUrl = "https://facebook.com/asdpqwdqwdqwd",
                    Zone = "Ajman",
                    Name = "Alrahma"
                }
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
                PhoneNumberConfirmed = true,
                Zone = "Ras al-Khaimah",
                Center = new CenterEntity
                {
                    Id = Guid.NewGuid(),
                    Capacity = 150,
                    Gender = "Female",
                    IsEnabled = true,
                    LocationUrl = "https://twitter.com/asdmqwdmqmpwdqw",
                    Zone = "Fujairah",
                    Name = "Loma By"
                }
            },
            new UserEntity
            {
                Id = Guid.NewGuid(),
                FirstName = "Admin",
                LastName = "Center 4",
                NationalId = "1212-56331-552",
                Email = "admincenter4@test.com",
                UserName = "admincenter4@test.com",
                Gender = "Male",
                PhoneNumber = "012045958236",
                PhoneNumberConfirmed = true,
                Zone = "Dubai"
            },
            new UserEntity
            {
                Id = Guid.NewGuid(),
                FirstName = "Admin",
                LastName = "Center 5",
                NationalId = "515-25565-45",
                Email = "admincenter5@test.com",
                UserName = "admincenter5@test.com",
                Gender = "Female",
                PhoneNumber = "012085413112",
                PhoneNumberConfirmed = true,
                Zone = "Sharjah"
            }
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
                PhoneNumberConfirmed = true,
                Zone="Ras al-Khaimah"
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
                PhoneNumberConfirmed = true,
                Zone = "Ras al-Khaimah"
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
                PhoneNumberConfirmed = true,
                Zone = "Umm al-Quwain"
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
            PhoneNumberConfirmed = true,
            Zone = "Sharjah"
        };

        await userManager.CreateAsync(reviewerUser, "Pa$$w0rd");
        await userManager.AddToRoleAsync(reviewerUser, Constants.Roles.Reviewer);

        var zones = new[]
        {
            "Abu Dhabi",
            "Ajman",
            "Dubai",
            "Fujairah",
            "Ras al-Khaimah",
            "Sharjah",
            "Umm al-Quwain"
        };

        // Seeding some fake users for testing.
        var fakeStudents = new Faker<UserEntity>("ar")
            .RuleFor(u => u.Id, f => Guid.NewGuid())
            .RuleFor(u => u.FirstName, f => f.Person.FirstName)
            .RuleFor(u => u.LastName, f => f.Person.LastName)
            .RuleFor(s => s.Zone, f => f.Random.ArrayElement(zones))
            .RuleFor(u => u.Email, f => f.Person.Email)
            .RuleFor(u => u.UserName, f => f.Person.UserName)
            .RuleFor(u => u.PhoneNumber, f => f.Person.Phone)
            .RuleFor(u => u.PhoneNumberConfirmed, f => true)
            .RuleFor(u => u.NationalId, f => f.Person.Random.AlphaNumeric(15))
            .RuleFor(u => u.Gender, f => (new[] { "Female", "Male" })[new Random().Next(2)])
            .Generate(50);

        foreach (var student in fakeStudents)
        {
            await userManager.CreateAsync(student, "Pa$$w0rd");
            await userManager.AddToRoleAsync(student, Constants.Roles.Student);
        }

    }

    public static async Task SeedSubjectsAndExamDates(CentersDbContext context)
    {
        ArgumentNullException.ThrowIfNull(nameof(context));

        if (await context.ExamDates.AnyAsync())
        {
            return;
        }

        var fakeSubjects = new Faker<SubjectEntity>("ar")
            .RuleFor(s => s.Id, f => Guid.NewGuid())
            .RuleFor(s => s.Name, f => f.Hacker.Verb())
            .RuleFor(s => s.Description, f => f.Lorem.Paragraph(4))
            .Generate(50);

        context.Subjects.AddRange(fakeSubjects);
        await context.SaveChangesAsync();

        var fakeExamDates = new Faker<ExamDateEntity>()
            .RuleFor(s => s.Id, f => Guid.NewGuid())
            .RuleFor(s => s.Date, f => f.Date.Between(DateTime.UtcNow.AddYears(-1), DateTime.UtcNow.AddYears(1)))
            .RuleFor(s => s.OpeningDate, f => f.Date.Between(DateTime.UtcNow.AddYears(-2), DateTime.UtcNow.AddYears(2)))
            .RuleFor(s => s.ClosingDate, f => f.Date.Between(DateTime.UtcNow.AddYears(-3), DateTime.UtcNow.AddYears(3)))
            .Generate(50);

        context.ExamDates.AddRange(fakeExamDates);
        await context.SaveChangesAsync();
    }

}
