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
    public class MembershipsControllerTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Sell_WithInsufficientAmount_ReturnsViewWithError()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var controller = new MembershipsController(context);

            var client = new Client
            {
                FullName = "Сидорова Мария Викторовна",
                Phone = "+79123456789",
                Status = ClientStatus.Potential
            };
            context.Clients.Add(client);

            var membershipType = new MembershipType
            {
                Name = "Месячный 'Все включено'",
                Price = 5000,
                Category = MembershipCategory.TimeBased,
                DurationDays = 30,
                IsActive = true
            };
            context.MembershipTypes.Add(membershipType);
            await context.SaveChangesAsync();

            var model = new SellMembershipViewModel
            {
                ClientId = client.ClientId,
                MembershipTypeId = membershipType.MembershipTypeId,
                StartDate = DateTime.Today,
                Amount = 4000, // Меньше стоимости
                PaymentMethod = PaymentMethod.Cash
            };

            // Act
            var result = await controller.Sell(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public async Task Sell_WithValidData_RedirectsToIndex()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var controller = new MembershipsController(context);

            var client = new Client
            {
                FullName = "Сидорова Мария Викторовна",
                Phone = "+79123456789",
                Status = ClientStatus.Potential
            };
            context.Clients.Add(client);

            var membershipType = new MembershipType
            {
                Name = "Месячный 'Все включено'",
                Price = 5000,
                Category = MembershipCategory.TimeBased,
                DurationDays = 30,
                IsActive = true
            };
            context.MembershipTypes.Add(membershipType);
            await context.SaveChangesAsync();

            var model = new SellMembershipViewModel
            {
                ClientId = client.ClientId,
                MembershipTypeId = membershipType.MembershipTypeId,
                StartDate = DateTime.Today,
                Amount = 5000,
                PaymentMethod = PaymentMethod.Cash
            };

            // Act
            var result = await controller.Sell(model);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }
    }
}

