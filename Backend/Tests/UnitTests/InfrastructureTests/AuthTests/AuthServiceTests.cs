using AutoFixture;
using Core.Dtos;
using Core.Dtos.Settings;
using Core.Exceptions;
using Core.Interfaces;
using Core.Settings;
using Infrastructure.Services;
using Infrastructure.Services.Portal;
using Microsoft.Extensions.Options;
using Moq;

namespace InfrastructureTests.AuthTests
{
    [TestFixture]
    public class AuthServiceTests
    {
        private readonly IUserMapper _mapper = new UserMapper(
                new PortalMapper(),
                new UniversityMapper());
        private IEncryptor _encryptor;
        private IJwtTokenGenerator _jwt;


        private readonly JwtSettings jwtSettings = new()
        {
            Key = "asdadadaisodASifimallivewhyareumyremedyosajfa",
            Issuer = "lukaco",
            Audience = "tqven",
            AccessTokenExpiration = 5,
            RefreshTokenExpiration = 30
        };
        private readonly EncryptorSettings encryptorSettings = new()
        {
            Key = Convert.FromBase64String("MDEyMzQ1Njc4OWFiY2RlZjAxMjM0NTY3ODlhYmNkZWY="),
            IV = Convert.FromBase64String("bXlJbml1VmVjdG9yMTIzNA==")
        };


        [OneTimeSetUp]
        public void BeforeAll()
        {
            var encryptorOptions = Options.Create(encryptorSettings);
            _encryptor = new Encryptor(encryptorOptions);

            var generatorOptions = Options.Create(jwtSettings);
            _jwt = new JwtTokenGenerator(generatorOptions);
        }

        private AuthService CreateAuthService(IUserRepo repo)
            => new(repo, _mapper, _encryptor, _jwt);

        [Test]
        public async Task LoginAsync_Success()
        {
            var fixture = new Fixture();
            var login = fixture.Create<LoginRequest>();

            var user = _mapper.ToUser(login);
            user.Password = _encryptor.Encrypt(login.Password);

            var mock = new Mock<IUserRepo>();
            mock.Setup(r => r.GetByEmailAsync(login.Email))
                .ReturnsAsync(() => user);
            var auth = CreateAuthService(mock.Object);

            var res = await auth.LoginAsync(login);

            Assert.That(res, Is.Not.Null);
            mock.Verify(r => r.GetByEmailAsync(login.Email), Times.Once());
        }


        [Test]
        public async Task LoginAsync_NotFound_Throws()
        {
            var fixture = new Fixture();
            var login = fixture.Create<LoginRequest>();

            var mock = new Mock<IUserRepo>();
            mock.Setup(r => r.GetByEmailAsync(login.Email))
                .ReturnsAsync(() => null);
            var auth = CreateAuthService(mock.Object);

            Assert.ThrowsAsync<UserNotFoundException>(async () =>
                    await auth.LoginAsync(login));
            mock.Verify(r => r.GetByEmailAsync(login.Email), Times.Once());
        }


        [Test]
        public async Task LoginAsync_WrongPassword_Throws()
        {
            var fixture = new Fixture();
            var login = fixture.Create<LoginRequest>();

            var user = _mapper.ToUser(login);
            user.Password = _encryptor.Encrypt(login.Password + "hehe");

            var mock = new Mock<IUserRepo>();
            mock.Setup(r => r.GetByEmailAsync(login.Email))
                .ReturnsAsync(() => user);
            var auth = CreateAuthService(mock.Object);

            Assert.ThrowsAsync<UserWrongCredentialsException>(async () =>
                    await auth.LoginAsync(login));
            mock.Verify(r => r.GetByEmailAsync(login.Email), Times.Once());
        }

        [Test]
        public async Task RegisterAsync_Success()
        {
            var fixture = new Fixture();
            var register = fixture.Create<RegisterRequest>();

            var mock = new Mock<IUserRepo>();
            mock.Setup(r => r.GetByEmailAsync(register.Email))
                .ReturnsAsync(() => null);
            var auth = CreateAuthService(mock.Object);

            var res = await auth.RegisterAsync(register);

            Assert.That(res, Is.Not.Null);
            mock.Verify(r => r.GetByEmailAsync(register.Email), Times.Once());
        }


        [Test]
        public async Task RegisterAsync_Exists_Throws()
        {
            var fixture = new Fixture();
            var register = fixture.Create<RegisterRequest>();
            var user = _mapper.ToUser(register);

            var mock = new Mock<IUserRepo>();
            mock.Setup(r => r.GetByEmailAsync(register.Email))
                .ReturnsAsync(() => user);
            var auth = CreateAuthService(mock.Object);

            Assert.ThrowsAsync<UserAlreadyExistsException>(async () =>
                    await auth.RegisterAsync(register));
        }
    }
}
