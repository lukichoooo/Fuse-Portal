using System.Linq.Expressions;
using AutoFixture;
using Core.Dtos;
using Core.Entities;
using Core.Enums;
using Core.Exceptions;
using Core.Interfaces;
using Infrastructure.Services;
using Moq;

namespace InfrastructureTests.UniversityTests
{
    [TestFixture]
    public class UniversityServiceTests
    {
        private readonly IUniversityMapper _mapper = new UniversityMapper();
        private readonly IUserMapper _userMapper = new UserMapper();
        private static readonly Fixture _globalFixture = new();

        private IUniversityService CreateServiceReturns<T>(
                Expression<Func<IUniversityRepo, Task<T>>> exp, T returnValue)
        {
            var mock = new Mock<IUniversityRepo>();
            mock.Setup(exp).ReturnsAsync(returnValue);
            return new UniversityService(mock.Object, _mapper, _userMapper);
        }

        private IUniversityService CreateServiceThrows<T>(
                Expression<Func<IUniversityRepo, Task<T>>> exp, Exception e)
        {
            var mock = new Mock<IUniversityRepo>();
            mock.Setup(exp).ThrowsAsync(e);
            return new UniversityService(mock.Object, _mapper, _userMapper);
        }

        private IUniversityService CreateService(IUniversityRepo repo)
            => new UniversityService(repo, _mapper, _userMapper);

        private static University CreateUniversityById(int id)
        {
            return _globalFixture.Build<University>()
                .With(uni => uni.Id, id)
                .With(uni => uni.Users, [])
                .Create();
        }

        [Test]
        public async Task GetAsync_Success()
        {
            const int id = 5;
            var uni = CreateUniversityById(id);
            var service = CreateServiceReturns(r => r.GetAsync(id), uni);

            var res = await service.GetAsync(id);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Id, Is.EqualTo(id));
        }


        [Test]
        public async Task GetAsync_NotFound_Throws()
        {
            const int id = 5;
            var service = CreateServiceReturns(r => r.GetAsync(id), null);

            Assert.ThrowsAsync<UniversityNotFoundException>(async () =>
                    await service.GetAsync(id));
        }

        [TestCase(new int[] { })]
        [TestCase(new[] { 1, 5, 19 })]
        public async Task GetUsersPageAsync_Success(int[] ids)
        {
            List<User> users = ids.Select(id => new User { Id = id }).ToList();
            const int id = 5, lastId = -1, pageSize = 16;
            var service = CreateServiceReturns(r => r.GetUsersPageAsync(id, lastId, pageSize), users);

            var res = await service.GetUsersPageAsync(id, lastId, pageSize);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Select(u => u.Id).Order(),
                    Is.EquivalentTo(ids.Order()));
        }


        [Test]
        public async Task GetPageByNameAsync_Success()
        {
            List<University> unis =
            [
                new() {Name = "luka"},
                new() {Name = "LuKaa"},
                new() {Name = "XXLukAXX"},
            ];
            const int lastId = -1, pageSize = 16;
            const string name = "luka";
            var service = CreateServiceReturns(r => r.GetPageByNameAsync(name, lastId, pageSize), unis);

            var res = await service.GetPageByNameAsync(name, lastId, pageSize);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.ConvertAll(uni => uni.Name).Order(),
                    Is.EquivalentTo(unis.ConvertAll(uni => uni.Name).Order()));
        }

        private static UniRequestDto CreateUniInfo(int id = 1, string name = "Name", Address? address = null)
            => new()
            {
                Id = id,
                Name = name,
                Address = address ?? new() { CountryA3 = CountryCode.GEO, City = "Dubai", }
            };

        [Test]
        public async Task CreateAsync_Success()
        {
            const int id = 5;
            const string name = "uniName";
            var address = new Address { CountryA3 = CountryCode.GEO, City = "Dubai" };
            var info = CreateUniInfo(id, name, address);
            var uni = _mapper.ToUniversity(info);
            var mock = new Mock<IUniversityRepo>();
            mock.Setup(r => r.GetPageByNameAsync(name, It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync([]);
            mock.Setup(r => r.CreateAsync(It.IsAny<University>()))
                .ReturnsAsync(uni);
            var service = CreateService(mock.Object);

            UniDto res = await service.CreateAsync(info);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Name, Is.EqualTo(info.Name));
            mock.Verify(r => r.CreateAsync(It.IsAny<University>()), Times.Once);
        }


        [Test]
        public async Task CreateAsync_IdenticalExists_Throws()
        {
            const string name = "uniName";
            var info = CreateUniInfo(name: name);
            var uni = _mapper.ToUniversity(info);
            var mock = new Mock<IUniversityRepo>();
            mock.Setup(r => r.GetPageByNameAsync(name, It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync([uni]);
            var service = CreateService(mock.Object);

            Assert.ThrowsAsync<UniversityAlreadyExists>(async () =>
                    await service.CreateAsync(info));
        }

        [Test]
        public async Task UpdateAsync_Success()
        {
            const int id = 5;
            var info = CreateUniInfo(id: id);
            var uni = _mapper.ToUniversity(info);
            var service = CreateServiceReturns(r => r.UpdateAsync(It.IsAny<University>()), uni);

            var res = await service.UpdateAsync(info);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Id, Is.EqualTo(id));
        }


        [Test]
        public async Task UpdateAsync_NotFound_Throws()
        {
            const int id = 5;
            var info = CreateUniInfo(id: id);
            var uni = _mapper.ToUniversity(info);
            var service = CreateServiceThrows(r =>
                    r.UpdateAsync(It.IsAny<University>()),
                    new UniversityNotFoundException());

            Assert.ThrowsAsync<UniversityNotFoundException>(async () =>
                    await service.UpdateAsync(info));
        }



        [Test]
        public async Task DeleteByIdAsync_Success()
        {
            const int id = 5;
            var info = CreateUniInfo(id: id);
            var uni = _mapper.ToUniversity(info);
            var service = CreateServiceReturns(r => r.DeleteByIdAsync(id), uni);

            var res = await service.DeleteByIdAsync(id);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Id, Is.EqualTo(id));
        }


        [Test]
        public async Task DeleteByIdAsync_NotFound_Throws()
        {
            const int id = 5;
            var info = CreateUniInfo(id: id);
            var uni = _mapper.ToUniversity(info);
            var service = CreateServiceThrows(r =>
                    r.DeleteByIdAsync(id),
                    new UniversityNotFoundException());

            Assert.ThrowsAsync<UniversityNotFoundException>(async () =>
                    await service.DeleteByIdAsync(id));
        }
    }
}
