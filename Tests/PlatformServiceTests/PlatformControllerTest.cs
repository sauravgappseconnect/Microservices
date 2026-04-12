
using Microservices.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using PlatformService.Controllers;
using PlatformService.Data;
using PlatformService.DTO;

namespace PlatformServiceTests
{
    public class PlatformControllerTest
    {
        private PlatformServiceContext _platformDbContext;

        public PlatformControllerTest()
        {
            //Arrange
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<PlatformServiceContext>()
                .UseSqlite(connection)
                .Options;
            _platformDbContext = new PlatformServiceContext(options);
            _platformDbContext.Database.EnsureCreated();
            _platformDbContext.Platforms.Add(new PlatformService.Models.Platform
            {
                Id = Guid.NewGuid(),
                Cost = 0.0,
                Name = "Name",
                Publisher = "Test",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "",
                UpdatedAt = DateTime.UtcNow,
            });
            _platformDbContext.SaveChanges();
        }


        [Fact]
        public async Task GetAllPlatforms_Success()
        {
            //Arrange
            var mockMessageSender = new Mock<IMessageSender>();
            var fakeLogger = new FakeLogger<PlatformController>();
            var platformController = new PlatformController(_platformDbContext, mockMessageSender.Object, fakeLogger);

            //Act
            var response = await platformController.GetAllPlatforms(CancellationToken.None);

            //Asssert
            var result = Assert.IsType<OkObjectResult>(response);
            var apiResult = Assert.IsAssignableFrom<IEnumerable<PlatformModel>>(result.Value);
            Assert.True(apiResult?.Count() == 1);
        }
    }
}
