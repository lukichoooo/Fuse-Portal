using System.Linq.Expressions;
using Core.Dtos;
using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;
using Infrastructure.Services;
using Moq;

namespace InfrastructureTests.UserTests
{
    public class UserServiceTests
    {
        private readonly IUserMapper _mapper = new UserMapper();
        private readonly IUniversityMapper _uniMapper = new UniversityMapper();

        private UserService GetService<T>(
            Expression<Func<IUserRepo, Task<T>>> setupExpr,
            T returnValue)
        {
            var mock = new Mock<IUserRepo>();

            mock.Setup(setupExpr)
                .ReturnsAsync(returnValue);

            return new UserService(_mapper, _uniMapper, mock.Object);
        }

        private UserService GetService(IUserRepo repo)
            => new(_mapper, _uniMapper, repo);


        [TestCase(new int[] { })]
        [TestCase(new[] { 1, 3, 63 })]
        public async Task GetAllAsync_Success(int[] ids)
        {
            const int pageSize = 16;
            var users = ids.Select(id => new User { Id = id }).ToList();
            var service = GetService(r => r.GetAllPageAsync(-1, pageSize), users);

            var res = await service.GetAllPageAsync(-1, pageSize);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Select(u => u.Id),
                    Is.EqualTo(users.Select(u => u.Id).Order()));
        }

        [TestCase(new int[] { })]
        [TestCase(new[] { 1, 3, 63 })]
        public async Task SearchByNameAsync(int[] ids)
        {
            const int pageSize = 16;
            const string name = "masheri";
            var users = ids.Select(id => new User { Id = id }).ToList();
            var service = GetService(r => r.PageByNameAsync(name, -1, pageSize), users);

            var res = await service.PageByNameAsync(name, -1, pageSize);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Select(u => u.Id),
                    Is.EqualTo(users.Select(u => u.Id).Order()));
        }

        [Test]
        public async Task GetByIdAsync_Success()
        {
            const int id = 5;
            var user = new User { Id = id };
            var service = GetService(r => r.GetAsync(id), user);

            var res = await service.GetAsync(id);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Id, Is.EqualTo(id));
        }


        [Test]
        public async Task GetByIdAsync_NotFound_Throws()
        {
            const int id = 5;
            var user = new User { Id = id };
            var service = GetService(r => r.GetAsync(id), null);

            Assert.ThrowsAsync<UserNotFoundException>(async () =>
                    await service.GetAsync(id));
        }



        [Test]
        public async Task GetPrivateDtoById_Success()
        {
            const int id = 5;
            var user = new User { Id = id };
            var service = GetService(r => r.GetAsync(id), user);

            var res = await service.GetPrivateDtoById(id);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Id, Is.EqualTo(id));
        }


        [Test]
        public async Task GetPrivateDtoById_NotFound_Throws()
        {
            const int id = 5;
            var user = new User { Id = id };
            var service = GetService(r => r.GetAsync(id), null);

            Assert.ThrowsAsync<UserNotFoundException>(async () =>
                    await service.GetPrivateDtoById(id));
        }


        [Test]
        public async Task UpdateUserCredentialsAsync_Success()
        {
            var dto = new UserPrivateInfo(
                    id: 1,
                    name: "Pesho",
                    email: "pesho@gmail.com",
                    password: "123"
                );
            var user = _mapper.ToUser(dto);
            var service = GetService(r => r.UpdateUserCredentialsAsync(user), user);

            var res = service.UpdateUserCredentialsAsync(dto);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Id, Is.EqualTo(dto.Id));
        }

        [Test]
        public async Task UpdateUSerCredentialsAsync_NotFound_Throws()
        {
            const int id = 5;
            var dto = new UserPrivateInfo { Id = id };
            var mock = new Mock<IUserRepo>();
            mock.Setup(r => r.UpdateUserCredentialsAsync(It.IsAny<User>()))
                .ThrowsAsync(new UserNotFoundException());
            var service = GetService(mock.Object);

            Assert.ThrowsAsync<UserNotFoundException>(async () =>
                    await service.UpdateUserCredentialsAsync(dto));
            mock.Verify(r => r.UpdateUserCredentialsAsync(It.IsAny<User>()), Times.Once());
        }

        [Test]
        public async Task DeleteByIdAsync_Success()
        {
            const int id = 5;
            var dto = new UserPrivateInfo { Id = id };
            var user = _mapper.ToUser(dto);
            var service = GetService(r => r.DeleteByIdAsync(id), user);

            var res = await service.DeleteByIdAsync(id);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Id, Is.EqualTo(id));
        }


        [Test]
        public async Task DeleteByIdAsync_NotFound_THrows()
        {
            const int id = 5;
            var dto = new UserPrivateInfo { Id = id };
            var mock = new Mock<IUserRepo>();
            mock.Setup(r => r.DeleteByIdAsync(id))
                .ThrowsAsync(new UserNotFoundException());
            var service = GetService(mock.Object);

            Assert.ThrowsAsync<UserNotFoundException>(async () =>
                    await service.DeleteByIdAsync(id));
            mock.Verify(r => r.DeleteByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task GetUserDetailsAsync_Success()
        {
            const int id = 5;
            var dto = new UserPrivateInfo { Id = id };
            var user = _mapper.ToUser(dto);
            var service = GetService(r => r.GetUserDetailsAsync(id), user);

            var res = await service.GetUserDetailsAsync(id);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Id, Is.EqualTo(user.Id));
        }



        [Test]
        public async Task GetUserDetailsAsync_NotFound_Throws()
        {
            const int id = 5;
            var service = GetService(r => r.GetUserDetailsAsync(id), null);

            Assert.ThrowsAsync<UserNotFoundException>(() =>
                    service.GetUserDetailsAsync(id));
        }
    }
}
