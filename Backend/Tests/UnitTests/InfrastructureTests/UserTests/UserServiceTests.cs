using System.Linq.Expressions;
using AutoFixture;
using Core.Dtos;
using Core.Dtos.Settings;
using Core.Entities;
using Core.Enums;
using Core.Exceptions;
using Core.Interfaces;
using Infrastructure.Services;
using Microsoft.Extensions.Options;
using Moq;

namespace InfrastructureTests.UserTests
{
    public class UserServiceTests
    {
        private readonly IUserMapper _mapper = new UserMapper();
        private static readonly Fixture _globalFixture = new();

        private readonly EncryptorSettings _settings = new()
        {
            Key = Convert.FromBase64String("MDEyMzQ1Njc4OWFiY2RlZjAxMjM0NTY3ODlhYmNkZWY="),
            IV = Convert.FromBase64String("bXlJbml1VmVjdG9yMTIzNA==")
        };
        private UserService GetService<T>(
            Expression<Func<IUserRepo, Task<T>>> setupExpr,
            T returnValue)
        {
            var mock = new Mock<IUserRepo>();
            var options = Options.Create(_settings);
            var _encryptor = new Encryptor(options);

            mock.Setup(setupExpr)
                .ReturnsAsync(returnValue);

            return new UserService(_mapper, _encryptor, mock.Object);
        }

        private UserService GetService<T>(
            Expression<Func<IUserRepo, ValueTask<T>>> setupExpr,
            T returnValue)
        {
            var mock = new Mock<IUserRepo>();
            var options = Options.Create(_settings);
            var _encryptor = new Encryptor(options);

            mock.Setup(setupExpr)
                .ReturnsAsync(returnValue);

            return new UserService(_mapper, _encryptor, mock.Object);
        }



        private UserService GetService(IUserRepo repo)
        {
            var options = Options.Create(_settings);
            var _encryptor = new Encryptor(options);
            return new(_mapper, _encryptor, repo);
        }

        private static User CreateUserById(int id)
        {
            return _globalFixture.Build<User>()
                .With(u => u.Id, id)
                .With(u => u.Universities, [])
                .With(u => u.Faculties, [])
                .Create();
        }


        [TestCase(new int[] { })]
        [TestCase(new[] { 1, 3, 63 })]
        public async Task GetAllAsync_Success(int[] ids)
        {
            const int pageSize = 16;
            var users = ids.Select(id => CreateUserById(id)).ToList();
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
            var users = ids.Select(id => CreateUserById(id)).ToList();
            var service = GetService(r => r.PageByNameAsync(name, -1, pageSize), users);

            var res = await service.GetPageByNameAsync(name, -1, pageSize);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Select(u => u.Id),
                    Is.EqualTo(users.Select(u => u.Id).Order()));
        }

        [Test]
        public async Task GetByIdAsync_Success()
        {
            const int id = 5;
            var user = CreateUserById(id);
            var service = GetService(r => r.GetByIdAsync(id), user);

            var res = await service.GetByIdAsync(id);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Id, Is.EqualTo(id));
        }


        [Test]
        public async Task GetByIdAsync_NotFound_Throws()
        {
            const int id = 5;
            var user = CreateUserById(id);
            var service = GetService(r => r.GetByIdAsync(id), null);

            Assert.ThrowsAsync<UserNotFoundException>(async () =>
                    await service.GetByIdAsync(id));
        }



        [Test]
        public async Task GetPrivateDtoById_Success()
        {
            const int id = 5;
            var user = CreateUserById(id);
            var service = GetService(r => r.GetByIdAsync(id), user);

            var res = await service.GetPrivateDtoById(id);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Id, Is.EqualTo(id));
        }


        [Test]
        public async Task GetPrivateDtoById_NotFound_Throws()
        {
            const int id = 5;
            var user = CreateUserById(id);
            var service = GetService(r => r.GetByIdAsync(id), null);

            Assert.ThrowsAsync<UserNotFoundException>(async () =>
                    await service.GetPrivateDtoById(id));
        }

        private UserPrivateDto CreateUserPrivateDto(int? id = null,
                        string? name = null,
                        string? email = null,
                        string? password = null,
                        Address? address = null
                    )
            => new()
            {
                Id = id ?? 1,
                Name = name ?? string.Empty,
                Email = email ?? string.Empty,
                Password = password ?? string.Empty,
                Address = address ?? new()
                {
                    City = "NY",
                    CountryA3 = CountryCode.GEO
                }
            };


        [Test]
        public async Task UpdateUserCredentialsAsync_Success()
        {
            var dto = CreateUserPrivateDto();
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
            var dto = CreateUserPrivateDto(id: id);
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
            var dto = CreateUserPrivateDto(id: id);
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
            var dto = CreateUserPrivateDto(id: id);
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
            var dto = CreateUserPrivateDto(id: id);
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
