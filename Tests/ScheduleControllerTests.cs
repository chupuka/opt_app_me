using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProForm.Controllers;
using ProForm.Data;
using ProForm.Models;
using System;
using System.Threading.Tasks;

namespace ProForm.Tests
{
    public class ScheduleControllerTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Create_WithPastDate_ReturnsViewWithError()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var controller = new ScheduleController(context);

            var fitnessClass = new FitnessClass
            {
                Type = ClassType.Group,
                Title = "Йога для начинающих",
                StartTime = DateTime.Today.AddDays(-1), // Прошлая дата
                EndTime = DateTime.Today.AddDays(-1).AddHours(1),
                MaxParticipants = 15
            };

            // Act
            var result = await controller.Create(fitnessClass);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public async Task Create_WithZeroMaxParticipants_ReturnsViewWithError()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var controller = new ScheduleController(context);

            var fitnessClass = new FitnessClass
            {
                Type = ClassType.Group,
                Title = "Йога для начинающих",
                StartTime = DateTime.Today.AddDays(1),
                EndTime = DateTime.Today.AddDays(1).AddHours(1),
                MaxParticipants = 0 // Недопустимое значение
            };

            // Act
            var result = await controller.Create(fitnessClass);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public async Task RegisterClient_WithoutActiveMembership_ReturnsError()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var controller = new ScheduleController(context);

            var client = new Client
            {
                FullName = "Тестовый клиент",
                Phone = "+79123456789",
                Status = ClientStatus.Potential
            };
            context.Clients.Add(client);

            var fitnessClass = new FitnessClass
            {
                Type = ClassType.Group,
                Title = "Йога",
                StartTime = DateTime.Today.AddDays(1),
                EndTime = DateTime.Today.AddDays(1).AddHours(1),
                MaxParticipants = 15
            };
            context.FitnessClasses.Add(fitnessClass);
            await context.SaveChangesAsync();

            // Act
            var result = await controller.RegisterClient(fitnessClass.ClassId, client.ClientId);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.NotNull(jsonResult.Value);
        }
    }
}

