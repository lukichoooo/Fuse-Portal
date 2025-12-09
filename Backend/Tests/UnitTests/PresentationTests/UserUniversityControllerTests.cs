using AutoFixture;
using Core.Dtos;
using Core.Dtos.Settings.Presentation;
using Core.Entities.JoinTables;
using Core.Interfaces.UserUniversityTable;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using Presentation.Controllers;

namespace PresentationTests
{
    public class UserUniversityControllerTests
    {
        private readonly Fixture _fix = new();

        [OneTimeSetUp]
        public void BeforeAll()
        {
            _fix.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        private static readonly ControllerSettings _settings = new()
        {
            DefaultPageSize = 16,
            SmallPageSize = 8,
            BigPageSize = 32
        };

        private static UserUniversityController CreateController(IUserUniversityService service)
            => new(service, Options.Create(_settings));

        [TestCase(null, null)]
        [TestCase(0, null)]
        public async Task GetUsersByUniIdPageAsync_Success(int? lastId, int? pageSize)
        {
            var uniId = _fix.Create<int>();
            var users = _fix.CreateMany<UserDto>().ToList();
            var serviceMock = new Mock<IUserUniversityService>();
            serviceMock.Setup(s => s.GetUsersByUniIdPageAsync(
                        uniId, lastId, It.IsAny<int>()))
                    .ReturnsAsync(users);

            var controller = CreateController(serviceMock.Object);

            var rv = await controller.GetUsersByUniIdPageAsync(uniId, lastId, pageSize);
            var res = rv.Result as OkObjectResult;

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Value, Is.EqualTo(users));
        }


        [TestCase(null, null)]
        [TestCase(0, null)]
        public async Task GetUnisPageForUserAsync_Success(int? lastId, int? pageSize)
        {
            var userId = _fix.Create<int>();
            var unis = _fix.CreateMany<UniDto>().ToList();
            var serviceMock = new Mock<IUserUniversityService>();
            serviceMock.Setup(s => s.GetUnisPageForUserIdAsync(
                        userId, lastId, It.IsAny<int>()))
                    .ReturnsAsync(unis);

            var controller = CreateController(serviceMock.Object);

            var rv = await controller.GetUnisPageForUserIdAsync(userId, lastId, pageSize);
            var res = rv.Result as OkObjectResult;

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Value, Is.EqualTo(unis));
        }

        [Test]
        public async Task AddUserToUniversity_Success()
        {
            const int uniId = 5;
            var expected = new UserUniversity { UserId = 10, UniversityId = uniId };

            var serviceMock = new Mock<IUserUniversityService>();
            serviceMock
                .Setup(s => s.AddCurrentUserToUniversityAsync(uniId))
                .ReturnsAsync(expected);

            var controller = CreateController(serviceMock.Object);

            var rv = await controller.AddUserToUniversityAsync(uniId);
            var okResult = rv.Result as OkObjectResult;

            Assert.Multiple(() =>
            {
                Assert.That(okResult, Is.Not.Null);
                Assert.That(okResult!.Value, Is.EqualTo(expected));
            });

            serviceMock.Verify(s => s.AddCurrentUserToUniversityAsync(uniId), Times.Once);
        }

        [Test]
        public async Task RemoveUserFromUniversity_Success()
        {
            const int uniId = 5;
            var expected = new UserUniversity { UserId = 10, UniversityId = uniId };

            var serviceMock = new Mock<IUserUniversityService>();
            serviceMock
                .Setup(s => s.RemoveCurrentUserFromUniversityAsync(uniId))
                .ReturnsAsync(expected);

            var controller = CreateController(serviceMock.Object);

            var rv = await controller.RemoveUserFromUniversityAsync(uniId);
            var okResult = rv.Result as OkObjectResult;

            Assert.Multiple(() =>
            {
                Assert.That(okResult, Is.Not.Null);
                Assert.That(okResult!.Value, Is.EqualTo(expected));
            });

            serviceMock.Verify(s => s.RemoveCurrentUserFromUniversityAsync(uniId), Times.Once);
        }

    }
}
