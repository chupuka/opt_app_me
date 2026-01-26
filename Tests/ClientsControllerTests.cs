using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProForm.Controllers;
using ProForm.Data;
using ProForm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProForm.Tests
{
    public class ClientsControllerTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithListOfClients()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var controller = new ClientsController(context);

            // Добавляем тестового клиента
            var client = new Client
            {
                FullName = "Сидорова Мария Викторовна",
                Phone = "+79123456789",
                Email = "maria.sidorova@example.com",
                Status = ClientStatus.Potential
            };
            context.Clients.Add(client);
            await context.SaveChangesAsync();

            // Act
            var result = await controller.Index(null, null, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Client>>(viewResult.Model);
            Assert.Single(model);
        }

        [Fact]
        public async Task Create_WithValidData_RedirectsToIndex()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var controller = new ClientsController(context);

            var client = new Client
            {
                FullName = "Сидорова Мария Викторовна",
                DateOfBirth = new DateTime(1990, 3, 15),
                Phone = "+79123456789",
                Email = "maria.sidorova@example.com",
                Status = ClientStatus.Potential
            };

            // Act
            var result = await controller.Create(client);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Delete_ClientWithActiveMembership_ShowsErrorMessage()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var controller = new ClientsController(context);

            var client = new Client
            {
                FullName = "Тестовый клиент",
                Phone = "+79123456789",
                Status = ClientStatus.Active
            };
            context.Clients.Add(client);
            await context.SaveChangesAsync();

            var membershipType = new MembershipType
            {
                Name = "Тестовый абонемент",
                Price = 5000,
                Category = MembershipCategory.TimeBased,
                DurationDays = 30
            };
            context.MembershipTypes.Add(membershipType);
            await context.SaveChangesAsync();

            var membership = new Membership
            {
                ClientId = client.ClientId,
                MembershipTypeId = membershipType.MembershipTypeId,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(30),
                IsActive = true
            };
            context.Memberships.Add(membership);
            await context.SaveChangesAsync();

            // Act
            var result = await controller.Delete(client.ClientId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.ViewData["ErrorMessage"]);
        }
    }
}

