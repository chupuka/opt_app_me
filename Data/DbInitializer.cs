using Microsoft.AspNetCore.Identity;
using ProForm.Models;

namespace ProForm.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Инициализация ролей (если еще не созданы)
            await InitializeRolesAsync(roleManager);

            // Инициализация пользователей
            await InitializeUsersAsync(userManager);

            // Инициализация тестовых данных
            await InitializeTestDataAsync(context);
        }

        private static async Task InitializeRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "Manager", "Trainer" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private static async Task InitializeUsersAsync(UserManager<ApplicationUser> userManager)
        {
            // 1. Администратор
            if (await userManager.FindByEmailAsync("admin@proform.ru") == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@proform.ru",
                    FullName = "Администратор Системы",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(admin, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }

            // 2. Руководитель (Manager)
            if (await userManager.FindByEmailAsync("manager@proform.ru") == null)
            {
                var manager = new ApplicationUser
                {
                    UserName = "manager",
                    Email = "manager@proform.ru",
                    FullName = "Иванов Иван Иванович",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(manager, "Manager123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(manager, "Manager");
                }
            }

            // 3-7. Тренеры (5 штук)
            var trainers = new[]
            {
                new { UserName = "trainer1", Email = "trainer1@proform.ru", FullName = "Петров Петр Петрович", Password = "Trainer123!" },
                new { UserName = "trainer2", Email = "trainer2@proform.ru", FullName = "Сидоров Сидор Сидорович", Password = "Trainer123!" },
                new { UserName = "trainer3", Email = "trainer3@proform.ru", FullName = "Кузнецов Кузьма Кузьмич", Password = "Trainer123!" },
                new { UserName = "trainer4", Email = "trainer4@proform.ru", FullName = "Смирнова Анна Сергеевна", Password = "Trainer123!" },
                new { UserName = "trainer5", Email = "trainer5@proform.ru", FullName = "Волкова Мария Дмитриевна", Password = "Trainer123!" }
            };

            foreach (var trainerData in trainers)
            {
                if (await userManager.FindByEmailAsync(trainerData.Email) == null)
                {
                    var trainer = new ApplicationUser
                    {
                        UserName = trainerData.UserName,
                        Email = trainerData.Email,
                        FullName = trainerData.FullName,
                        EmailConfirmed = true
                    };
                    var result = await userManager.CreateAsync(trainer, trainerData.Password);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(trainer, "Trainer");
                    }
                }
            }
        }

        private static async Task InitializeTestDataAsync(ApplicationDbContext context)
        {
            // Проверяем, есть ли уже данные
            if (context.Clients.Any())
            {
                return; // Данные уже инициализированы
            }

            // 1. Клиенты (минимум 3)
            var clients = new[]
            {
                new Client
                {
                    FullName = "Соколов Алексей Владимирович",
                    DateOfBirth = new DateTime(1990, 5, 15),
                    Phone = "+7 (999) 123-45-67",
                    Email = "sokolov@example.com",
                    Status = ClientStatus.Active
                },
                new Client
                {
                    FullName = "Новикова Елена Александровна",
                    DateOfBirth = new DateTime(1985, 8, 22),
                    Phone = "+7 (999) 234-56-78",
                    Email = "novikova@example.com",
                    Status = ClientStatus.Active
                },
                new Client
                {
                    FullName = "Морозов Дмитрий Сергеевич",
                    DateOfBirth = new DateTime(1992, 3, 10),
                    Phone = "+7 (999) 345-67-89",
                    Email = "morozov@example.com",
                    Status = ClientStatus.Potential
                },
                new Client
                {
                    FullName = "Лебедева Ольга Игоревна",
                    DateOfBirth = new DateTime(1988, 11, 5),
                    Phone = "+7 (999) 456-78-90",
                    Email = "lebedeva@example.com",
                    Status = ClientStatus.Active
                }
            };
            context.Clients.AddRange(clients);
            await context.SaveChangesAsync();

            // 2. Тренеры (5 штук) - связываем с пользователями
            var trainers = new[]
            {
                new Trainer
                {
                    FullName = "Петров Петр Петрович",
                    Specialization = "Силовые тренировки, Бодибилдинг",
                    Phone = "+7 (999) 111-11-11",
                    Email = "trainer1@proform.ru",
                    Status = TrainerStatus.Active
                },
                new Trainer
                {
                    FullName = "Сидоров Сидор Сидорович",
                    Specialization = "Кардио, Функциональный тренинг",
                    Phone = "+7 (999) 222-22-22",
                    Email = "trainer2@proform.ru",
                    Status = TrainerStatus.Active
                },
                new Trainer
                {
                    FullName = "Кузнецов Кузьма Кузьмич",
                    Specialization = "Йога, Пилатес, Стретчинг",
                    Phone = "+7 (999) 333-33-33",
                    Email = "trainer3@proform.ru",
                    Status = TrainerStatus.Active
                },
                new Trainer
                {
                    FullName = "Смирнова Анна Сергеевна",
                    Specialization = "Аэробика, Танцевальный фитнес",
                    Phone = "+7 (999) 444-44-44",
                    Email = "trainer4@proform.ru",
                    Status = TrainerStatus.Active
                },
                new Trainer
                {
                    FullName = "Волкова Мария Дмитриевна",
                    Specialization = "Кроссфит, HIIT тренировки",
                    Phone = "+7 (999) 555-55-55",
                    Email = "trainer5@proform.ru",
                    Status = TrainerStatus.Active
                }
            };
            context.Trainers.AddRange(trainers);
            await context.SaveChangesAsync();

            // 3. Типы абонементов (минимум 3)
            var membershipTypes = new[]
            {
                new MembershipType
                {
                    Name = "Базовый",
                    Price = 3000m,
                    Category = MembershipCategory.TimeBased,
                    DurationDays = 30,
                    VisitCount = null,
                    AvailableServices = "Тренажерный зал, Групповые занятия",
                    IsActive = true
                },
                new MembershipType
                {
                    Name = "Стандарт",
                    Price = 5000m,
                    Category = MembershipCategory.TimeBased,
                    DurationDays = 30,
                    VisitCount = null,
                    AvailableServices = "Тренажерный зал, Групповые занятия, Персональные тренировки (2 раза)",
                    IsActive = true
                },
                new MembershipType
                {
                    Name = "Премиум",
                    Price = 8000m,
                    Category = MembershipCategory.TimeBased,
                    DurationDays = 30,
                    VisitCount = null,
                    AvailableServices = "Тренажерный зал, Групповые занятия, Персональные тренировки (без ограничений), Сауна",
                    IsActive = true
                },
                new MembershipType
                {
                    Name = "Пакет 10 посещений",
                    Price = 4000m,
                    Category = MembershipCategory.VisitBased,
                    DurationDays = 60,
                    VisitCount = 10,
                    AvailableServices = "Тренажерный зал, Групповые занятия",
                    IsActive = true
                }
            };
            context.MembershipTypes.AddRange(membershipTypes);
            await context.SaveChangesAsync();

            // 4. Абонементы (минимум 3)
            var today = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Unspecified);
            var memberships = new[]
            {
                new Membership
                {
                    ClientId = clients[0].ClientId,
                    MembershipTypeId = membershipTypes[1].MembershipTypeId,
                    StartDate = today.AddDays(-10),
                    EndDate = today.AddDays(20),
                    RemainingVisits = null,
                    IsActive = true
                },
                new Membership
                {
                    ClientId = clients[1].ClientId,
                    MembershipTypeId = membershipTypes[2].MembershipTypeId,
                    StartDate = today.AddDays(-5),
                    EndDate = today.AddDays(25),
                    RemainingVisits = null,
                    IsActive = true
                },
                new Membership
                {
                    ClientId = clients[2].ClientId,
                    MembershipTypeId = membershipTypes[3].MembershipTypeId,
                    StartDate = today.AddDays(-15),
                    EndDate = today.AddDays(45),
                    RemainingVisits = 7,
                    IsActive = true
                },
                new Membership
                {
                    ClientId = clients[3].ClientId,
                    MembershipTypeId = membershipTypes[0].MembershipTypeId,
                    StartDate = today.AddDays(-20),
                    EndDate = today.AddDays(10),
                    RemainingVisits = null,
                    IsActive = true
                }
            };
            context.Memberships.AddRange(memberships);
            await context.SaveChangesAsync();

            // 5. Занятия (минимум 3)
            var fitnessClasses = new[]
            {
                new FitnessClass
                {
                    Type = ClassType.Group,
                    Title = "Силовая тренировка",
                    TrainerId = trainers[0].TrainerId,
                    Hall = "Зал 1",
                    StartTime = DateTime.SpecifyKind(today.AddDays(1).AddHours(10), DateTimeKind.Unspecified),
                    EndTime = DateTime.SpecifyKind(today.AddDays(1).AddHours(11), DateTimeKind.Unspecified),
                    MaxParticipants = 15
                },
                new FitnessClass
                {
                    Type = ClassType.Group,
                    Title = "Йога для начинающих",
                    TrainerId = trainers[2].TrainerId,
                    Hall = "Зал 2",
                    StartTime = DateTime.SpecifyKind(today.AddDays(1).AddHours(18), DateTimeKind.Unspecified),
                    EndTime = DateTime.SpecifyKind(today.AddDays(1).AddHours(19).AddMinutes(30), DateTimeKind.Unspecified),
                    MaxParticipants = 20
                },
                new FitnessClass
                {
                    Type = ClassType.Personal,
                    Title = "Персональная тренировка",
                    TrainerId = trainers[1].TrainerId,
                    Hall = "Зал 1",
                    StartTime = DateTime.SpecifyKind(today.AddDays(2).AddHours(14), DateTimeKind.Unspecified),
                    EndTime = DateTime.SpecifyKind(today.AddDays(2).AddHours(15), DateTimeKind.Unspecified),
                    MaxParticipants = 1
                },
                new FitnessClass
                {
                    Type = ClassType.Group,
                    Title = "Аэробика",
                    TrainerId = trainers[3].TrainerId,
                    Hall = "Зал 2",
                    StartTime = DateTime.SpecifyKind(today.AddDays(2).AddHours(19), DateTimeKind.Unspecified),
                    EndTime = DateTime.SpecifyKind(today.AddDays(2).AddHours(20), DateTimeKind.Unspecified),
                    MaxParticipants = 25
                },
                new FitnessClass
                {
                    Type = ClassType.Group,
                    Title = "Кроссфит",
                    TrainerId = trainers[4].TrainerId,
                    Hall = "Зал 1",
                    StartTime = DateTime.SpecifyKind(today.AddDays(3).AddHours(9), DateTimeKind.Unspecified),
                    EndTime = DateTime.SpecifyKind(today.AddDays(3).AddHours(10), DateTimeKind.Unspecified),
                    MaxParticipants = 12
                }
            };
            context.FitnessClasses.AddRange(fitnessClasses);
            await context.SaveChangesAsync();

            // 6. Регистрации на занятия (минимум 3)
            var registrations = new[]
            {
                new ClassRegistration
                {
                    ClassId = fitnessClasses[0].ClassId,
                    ClientId = clients[0].ClientId,
                    Attended = false,
                    RegistrationDate = today
                },
                new ClassRegistration
                {
                    ClassId = fitnessClasses[1].ClassId,
                    ClientId = clients[1].ClientId,
                    Attended = false,
                    RegistrationDate = today
                },
                new ClassRegistration
                {
                    ClassId = fitnessClasses[2].ClassId,
                    ClientId = clients[2].ClientId,
                    Attended = false,
                    RegistrationDate = today.AddDays(-1)
                },
                new ClassRegistration
                {
                    ClassId = fitnessClasses[0].ClassId,
                    ClientId = clients[3].ClientId,
                    Attended = false,
                    RegistrationDate = today
                }
            };
            context.ClassRegistrations.AddRange(registrations);
            await context.SaveChangesAsync();

            // 7. Платежи (минимум 3)
            var payments = new[]
            {
                new Payment
                {
                    ClientId = clients[0].ClientId,
                    MembershipId = memberships[0].MembershipId,
                    Amount = 5000m,
                    PaymentMethod = PaymentMethod.Card,
                    PaymentDate = DateTime.SpecifyKind(today.AddDays(-10), DateTimeKind.Unspecified),
                    Notes = "Оплата абонемента Стандарт"
                },
                new Payment
                {
                    ClientId = clients[1].ClientId,
                    MembershipId = memberships[1].MembershipId,
                    Amount = 8000m,
                    PaymentMethod = PaymentMethod.Card,
                    PaymentDate = DateTime.SpecifyKind(today.AddDays(-5), DateTimeKind.Unspecified),
                    Notes = "Оплата абонемента Премиум"
                },
                new Payment
                {
                    ClientId = clients[2].ClientId,
                    MembershipId = memberships[2].MembershipId,
                    Amount = 4000m,
                    PaymentMethod = PaymentMethod.Cash,
                    PaymentDate = DateTime.SpecifyKind(today.AddDays(-15), DateTimeKind.Unspecified),
                    Notes = "Оплата пакета 10 посещений"
                },
                new Payment
                {
                    ClientId = clients[3].ClientId,
                    MembershipId = memberships[3].MembershipId,
                    Amount = 3000m,
                    PaymentMethod = PaymentMethod.SBP,
                    PaymentDate = DateTime.SpecifyKind(today.AddDays(-20), DateTimeKind.Unspecified),
                    Notes = "Оплата абонемента Базовый"
                }
            };
            context.Payments.AddRange(payments);
            await context.SaveChangesAsync();

            // 8. Расписание тренеров
            var schedules = new List<TrainerSchedule>();
            foreach (var trainer in trainers)
            {
                // Понедельник - Пятница, 9:00 - 18:00
                for (int day = 1; day <= 5; day++)
                {
                    schedules.Add(new TrainerSchedule
                    {
                        TrainerId = trainer.TrainerId,
                        DayOfWeek = (DayOfWeek)day,
                        StartTime = new TimeSpan(9, 0, 0),
                        EndTime = new TimeSpan(18, 0, 0)
                    });
                }
            }
            context.TrainerSchedules.AddRange(schedules);
            await context.SaveChangesAsync();

            // 9. Заморозки абонементов (опционально, 2 записи)
            var freezes = new[]
            {
                new MembershipFreeze
                {
                    MembershipId = memberships[0].MembershipId,
                    FreezeStartDate = today.AddDays(5),
                    FreezeEndDate = today.AddDays(12),
                    Reason = "Командировка"
                },
                new MembershipFreeze
                {
                    MembershipId = memberships[3].MembershipId,
                    FreezeStartDate = today.AddDays(-5),
                    FreezeEndDate = today.AddDays(2),
                    Reason = "Болезнь"
                }
            };
            context.MembershipFreezes.AddRange(freezes);
            await context.SaveChangesAsync();
        }
    }
}
