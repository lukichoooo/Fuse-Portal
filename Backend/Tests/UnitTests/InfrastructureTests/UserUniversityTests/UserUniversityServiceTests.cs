using AutoFixture;
using Core.Entities;
using Core.Interfaces;
using Core.Interfaces.UserUniversityTable;
using Infrastructure.Services;
using Infrastructure.Services.Portal;
using Moq;

namespace InfrastructureTests.UserUniversityTests
{
    public class UserUniversityServiceTests
    {
        private readonly IUniversityMapper _mapper = new UniversityMapper();

        private readonly IUserMapper _userMapper = new UserMapper(
                new PortalMapper(), new UniversityMapper());

        private IUserUniversityService CreateService(IUserUniversityRepo repo)
            => new UserUniversityService(
                    repo,
                    new UserMapper(new PortalMapper(), new UniversityMapper()),
                    new UniversityMapper()
                    );



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

            var service = CreateService(mock.Object);

            var res = await service.GetUsersByUniIdPageAsync(id, lastId, pageSize);

            Assert.That(res, Is.Not.Null);
            Assert.That(
                    res.Select(u => u.Id).Order(),
                    Is.EquivalentTo(users.Select(u => u.Id).Order()
                        ));
        }
    }
}
