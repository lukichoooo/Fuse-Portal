using AutoFixture;
using Core.Entities;
using Core.Entities.JoinTables;
using Core.Interfaces;
using Core.Interfaces.Auth;
using Core.Interfaces.UserUniversityTable;
using Infrastructure.Services;
using Infrastructure.Services.Portal;
using Moq;

namespace InfrastructureTests.UserUniversityTests
{
    public class UserUniversityServiceTests
    {
        private const int DEFAULT_CONTEXT_ID = 992582;
        private readonly IUniversityMapper _uniMapper = new UniversityMapper();

        private readonly IUserMapper _userMapper = new UserMapper(
                new PortalMapper(), new UniversityMapper());

        private IUserUniversityService CreateService(IUserUniversityRepo repo)
        {
            var mock = new Mock<ICurrentContext>();
            mock.Setup(c => c.GetCurrentUserId())
                .Returns(DEFAULT_CONTEXT_ID);
            return new UserUniversityService(
                    repo,
                    new UserMapper(new PortalMapper(), new UniversityMapper()),
                    new UniversityMapper(),
                    mock.Object
                    );

        }

        [TestCase(0)]
        [TestCase(4)]
        public async Task GetUnisPageForUserAsync_Success(int repeatCount)
        {
            var fixture = new Fixture() { RepeatCount = repeatCount };
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            List<University> unis = fixture
                .CreateMany<University>()
                .ToList();
            const int userId = 5, pageSize = 16;
            int? lastId = null;

            var mock = new Mock<IUserUniversityRepo>();
            mock.Setup(r => r.GetUnisPageForUserIdAsync(userId, lastId, pageSize))
                .ReturnsAsync(unis);

            var service = CreateService(mock.Object);

            var res = await service.GetUnisPageForUserIdAsync(userId, lastId, pageSize);

            Assert.That(res, Is.Not.Null);
            Assert.That(
                    res.Select(u => u.Id).Order(),
                    Is.EquivalentTo(unis.Select(u => u.Id).Order()
                        ));
        }


        [TestCase(0)]
        [TestCase(4)]
        public async Task GetUsersPageAsync_Success(int repeatCount)
        {
            var fixture = new Fixture() { RepeatCount = repeatCount };
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            List<User> users = fixture.CreateMany<User>()
                .ToList();
            const int id = 5, pageSize = 16;
            int? lastId = null;

            var mock = new Mock<IUserUniversityRepo>();
            mock.Setup(r => r.GetUsersByUniIdPageAsync(id, lastId, pageSize))
                .ReturnsAsync(users);

            var sut = CreateService(mock.Object);

            var res = await sut.GetUsersByUniIdPageAsync(id, lastId, pageSize);

            Assert.That(res, Is.Not.Null);
            Assert.That(
                    res.Select(u => u.Id).Order(),
                    Is.EquivalentTo(users.Select(u => u.Id).Order()
                        ));
        }

        [Test]
        public async Task AddUserToUniversityAsync_CallsRepoCorrectly()
        {
            const int uniId = 10;
            var mock = new Mock<IUserUniversityRepo>();
            mock.Setup(r => r.AddUserToUniversityAsync(It.IsAny<UserUniversity>()))
                .ReturnsAsync((UserUniversity uu) => uu);
            var sut = CreateService(mock.Object);

            var res = await sut.AddCurrentUserToUniversityAsync(uniId);

            Assert.That(res, Is.Not.Null);
            mock.Verify(r =>
                r.AddUserToUniversityAsync(
                    It.Is<UserUniversity>(uu => uu.UserId == DEFAULT_CONTEXT_ID
                        && uu.UniversityId == uniId
                    )),
                Times.Once
            );
        }

        [Test]
        public async Task RemoveUserFromUniversityAsync_CallsRepoCorrectly()
        {
            const int uniId = 10;
            var returnValue = new UserUniversity
            {
                UserId = DEFAULT_CONTEXT_ID,
                UniversityId = uniId
            };
            var mock = new Mock<IUserUniversityRepo>();
            mock.Setup(r => r.RemoveUserFromUniversityAsync(DEFAULT_CONTEXT_ID, uniId))
                .ReturnsAsync(returnValue);
            var sut = CreateService(mock.Object);

            var res = await sut.RemoveCurrentUserFromUniversityAsync(uniId);

            Assert.That(res, Is.Not.Null);
            Assert.That(res, Is.EqualTo(returnValue));
            mock.Verify(r =>
                r.RemoveUserFromUniversityAsync(DEFAULT_CONTEXT_ID, uniId),
                Times.Once
            );
        }


    }
}
