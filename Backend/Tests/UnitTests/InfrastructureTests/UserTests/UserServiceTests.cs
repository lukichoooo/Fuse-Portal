using AutoFixture;
using Core.Dtos;
using Core.Dtos.Settings;
using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;
using Core.Interfaces.Auth;
using Infrastructure.Services;
using Infrastructure.Services.Portal;
using Microsoft.Extensions.Options;
using Moq;
using UnitTests;

namespace InfrastructureTests.UserTests
{
    public class UserServiceTests
    {
        private readonly IUserMapper _mapper = new UserMapper(
                new PortalMapper(),
                new UniversityMapper());

        private const int DEFAULT_CONTEXT_ID = 9845;

        private readonly EncryptorSettings _settings = new()
        {
            Key = Convert.FromBase64String("MDEyMzQ1Njc4OWFiY2RlZjAxMjM0NTY3ODlhYmNkZWY="),
            IV = Convert.FromBase64String("bXlJbml1VmVjdG9yMTIzNA==")
        };



        private UserService CreateService(
                IUserRepo repo,
                ICurrentContext? currentContext,
                IEncryptor? encryptor = null
                )
        {

            if (encryptor is null)
            {
                var options = Options.Create(_settings);
                encryptor = new Encryptor(options);
            }

            if (currentContext is null)
            {
                var mock = new Mock<ICurrentContext>();
                mock.Setup(c => c.GetCurrentUserId())
                    .Returns(DEFAULT_CONTEXT_ID);
                currentContext = mock.Object;
            }

            return new(_mapper, encryptor, repo, currentContext);
        }


        [TestCase(new int[] { })]
        [TestCase(new[] { 1, 3, 63 })]
        public async Task GetAllAsync_Success(int[] ids)
        {
            const int pageSize = 16;
            var users = ids.Select(id => HelperAutoFactory.CreateUser(id)).ToList();
            var repoMock = new Mock<IUserRepo>();
            repoMock.Setup(r => r.GetAllPageAsync(-1, pageSize))
                .ReturnsAsync(users);
            var service = CreateService(repoMock.Object, null);

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
            var users = ids.Select(id => HelperAutoFactory.CreateUser(id)).ToList();

            var repoMock = new Mock<IUserRepo>();
            repoMock.Setup(r => r.PageByNameAsync(name, -1, pageSize))
                .ReturnsAsync(users);
            var service = CreateService(repoMock.Object, null);

            var res = await service.GetPageByNameAsync(name, -1, pageSize);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Select(u => u.Id),
                    Is.EqualTo(users.Select(u => u.Id).Order()));
        }

        [Test]
        public async Task GetByIdAsync_Success()
        {
            const int id = 5;
            var user = HelperAutoFactory.CreateUser(id);

            var repoMock = new Mock<IUserRepo>();
            repoMock.Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(user);
            var service = CreateService(repoMock.Object, null);

            var res = await service.GetByIdAsync(id);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Id, Is.EqualTo(id));
        }


        [Test]
        public async Task GetByIdAsync_NotFound_Throws()
        {
            const int id = 5;
            var user = HelperAutoFactory.CreateUser(id);

            var repoMock = new Mock<IUserRepo>();
            repoMock.Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(() => null);

            var service = CreateService(repoMock.Object, null);

            Assert.ThrowsAsync<UserNotFoundException>(async () =>
                    await service.GetByIdAsync(id));
        }



        [Test]
        public async Task GetCurrentUserPrivateDto_Success()
        {
            const int id = 5;
            var user = HelperAutoFactory.CreateUser(id);

            var encryptorMock = new Mock<IEncryptor>();
            encryptorMock.Setup(e => e.Decrypt(It.IsAny<string>()))
                         .Returns("epicPassword");

            var repoMock = new Mock<IUserRepo>();
            repoMock.Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(user);

            var currContextMock = new Mock<ICurrentContext>();
            currContextMock.Setup(c => c.GetCurrentUserId())
                .Returns(id);

            var service = CreateService(
                    repoMock.Object,
                    currContextMock.Object,
                    encryptorMock.Object);

            var res = await service.GetCurrentUserPrivateDtoAsync();

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Id, Is.EqualTo(id));
        }




        [Test]
        public async Task GetPrivateDtoById_NotFound_Throws()
        {
            const int id = 5;
            var user = HelperAutoFactory.CreateUser(id);

            var repoMock = new Mock<IUserRepo>();
            repoMock.Setup(r => r.GetByIdAsync(id))
                .ReturnsAsync(() => null);

            var service = CreateService(repoMock.Object, null);

            Assert.ThrowsAsync<UserNotFoundException>(service.GetCurrentUserPrivateDtoAsync);
        }



        [Test]
        public async Task UpdateCurrentUserCredentialsAsync_Success()
        {
            var fixture = new Fixture();
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var user = fixture.Create<User>();
            var updateRequest = fixture.Create<UserUpdateRequest>();

            var userProps = typeof(User).GetProperties().ToDictionary(p => p.Name);
            foreach (var p in typeof(UserUpdateRequest).GetProperties())
            {
                if (userProps.TryGetValue(p.Name, out var targetProp) &&
                    targetProp.CanWrite &&
                    targetProp.PropertyType == p.PropertyType)
                {
                    var value = p.GetValue(updateRequest);
                    targetProp.SetValue(user, value);
                }
            }


            user.Id = DEFAULT_CONTEXT_ID;

            var currContextMock = new Mock<ICurrentContext>();
            currContextMock.Setup(c => c.GetCurrentUserId())
                .Returns(DEFAULT_CONTEXT_ID);

            var repoMock = new Mock<IUserRepo>();
            repoMock.Setup(r => r.UpdateUserCredentialsAsync(It.IsAny<User>()))
                .ReturnsAsync(user);

            var service = CreateService(repoMock.Object, currContextMock.Object);

            var res = await service.UpdateCurrentUserCredentialsAsync(updateRequest);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Id, Is.EqualTo(DEFAULT_CONTEXT_ID));
        }

        [Test]
        public async Task UpdateCurrentUserCredentialsAsync_NotFound_Throws()
        {
            var fixture = new Fixture();
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            int currentContextId = fixture.Create<int>();
            var request = fixture.Create<UserUpdateRequest>();
            var user = fixture.Create<User>();

            var repoMock = new Mock<IUserRepo>();
            repoMock.Setup(r => r.UpdateUserCredentialsAsync(It.IsAny<User>()))
                .ThrowsAsync(new UserNotFoundException());

            var currentContextMock = new Mock<ICurrentContext>();
            currentContextMock.Setup(c => c.GetCurrentUserId())
                .Returns(currentContextId);

            var service = CreateService(repoMock.Object, currentContextMock.Object);

            Assert.ThrowsAsync<UserNotFoundException>(async () =>
                    await service.UpdateCurrentUserCredentialsAsync(request));
            repoMock.Verify(r => r.UpdateUserCredentialsAsync(It.IsAny<User>()), Times.Once());
        }

        [Test]
        public async Task DeleteByIdAsync_Success()
        {
            const int id = 5;
            var dto = HelperAutoFactory.CreateUserPrivateDto(id: id);
            var user = _mapper.ToUser(dto);
            var repoMock = new Mock<IUserRepo>();
            repoMock.Setup(r => r.DeleteByIdAsync(id))
                .ReturnsAsync(user);
            var service = CreateService(repoMock.Object, null);

            var res = await service.DeleteByIdAsync(id);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Id, Is.EqualTo(id));
        }


        [Test]
        public async Task DeleteByIdAsync_NotFound_THrows()
        {
            const int id = 5;
            var dto = HelperAutoFactory.CreateUserPrivateDto(id: id);
            var mock = new Mock<IUserRepo>();
            mock.Setup(r => r.DeleteByIdAsync(id))
                .ThrowsAsync(new UserNotFoundException());
            var service = CreateService(mock.Object, null);

            Assert.ThrowsAsync<UserNotFoundException>(async () =>
                    await service.DeleteByIdAsync(id));
            mock.Verify(r => r.DeleteByIdAsync(id), Times.Once());
        }

        [Test]
        public async Task GetUserDetailsAsync_Success()
        {
            const int id = 5;
            var dto = HelperAutoFactory.CreateUserPrivateDto(id: id);
            var user = _mapper.ToUser(dto);
            var repoMock = new Mock<IUserRepo>();
            repoMock.Setup(r => r.GetUserDetailsByIdAsync(id))
                .ReturnsAsync(user);
            var service = CreateService(repoMock.Object, null);

            var res = await service.GetUserDetailsAsync(id);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Id, Is.EqualTo(user.Id));
        }



        [Test]
        public async Task GetUserDetailsAsync_NotFound_Throws()
        {
            const int id = 5;
            var repoMock = new Mock<IUserRepo>();
            repoMock.Setup(r => r.GetUserDetailsByIdAsync(id))
                .ReturnsAsync(() => null);
            var service = CreateService(repoMock.Object, null);

            Assert.ThrowsAsync<UserNotFoundException>(() =>
                    service.GetUserDetailsAsync(id));
        }
    }
}
