using System.Linq.Expressions;
using System.Security.Claims;
using AutoFixture;
using Core.Dtos;
using Core.Dtos.Settings.Presentation;
using Core.Exceptions;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using Presentation.Controllers;

namespace PresentationTests
{
    [TestFixture]
    public class UsercontrollerTests
    {

        private static readonly ControllerSettings _settings = new()
        {
            DefaultPageSize = 16,
            SmallPageSize = 8,
            BigPageSize = 32
        };

        private static UserController CreateControllerReturn<T>(
                Expression<Func<IUserService, Task<T>>> exp,
                T returnValue)
        {
            var mock = new Mock<IUserService>();
            mock.Setup(exp).ReturnsAsync(returnValue);
            return new UserController(mock.Object, Options.Create(_settings));
        }

        private static UserController CreateControllerThrows<T>(
                Expression<Func<IUserService, Task<T>>> exp,
                Exception e)
        {
            var mock = new Mock<IUserService>();
            mock.Setup(exp).ThrowsAsync(e);
            return new UserController(mock.Object, Options.Create(_settings));
        }


        private static UserController CreateControllerWithContext<T>(
            Expression<Func<IUserService, Task<T>>> exp,
            T returnValue, int id)
        {
            var mock = new Mock<IUserService>();
            mock.Setup(exp).ReturnsAsync(returnValue);

            var user = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim("id", id.ToString())
            ]));

            var httpContext = new DefaultHttpContext { User = user };

            return new UserController(mock.Object, Options.Create(_settings))
            {
                ControllerContext = new ControllerContext { HttpContext = httpContext }
            };
        }


        [TestCase(null, null)]
        [TestCase(0, null)]
        public async Task GetAllPageAsync_Success(int? lastId, int? pageSize)
        {
            var fixture = new Fixture();
            var users = fixture.CreateMany<UserDto>().ToList();
            var controller = CreateControllerReturn(s =>
                    s.GetAllPageAsync(lastId, It.IsAny<int>()),
                    users);

            var rv = await controller.GetAllPageAsync(lastId, pageSize);
            var res = rv.Result as OkObjectResult;

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Value, Is.EquivalentTo(users));
        }


        [Test]
        public async Task GetById_Success()
        {
            var fixture = new Fixture();
            var user = fixture.Create<UserDto>();
            var controller = CreateControllerReturn(s =>
                    s.GetByIdAsync(It.IsAny<int>()), user);

            var rv = await controller.GetByIdAsync(user.Id);
            var res = rv.Result as OkObjectResult;

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Value, Is.EqualTo(user));
        }



        [Test]
        public async Task GetById_NotFound_Throws()
        {
            const int id = 4;
            var controller = CreateControllerThrows(s =>
                    s.GetByIdAsync(It.IsAny<int>()), new UserNotFoundException());

            Assert.ThrowsAsync<UserNotFoundException>(async () =>
                    await controller.GetByIdAsync(id));
        }



        [TestCase(null, null)]
        [TestCase(int.MinValue, null)]
        public async Task PageByName_Success(int? lastId, int? pageSize)
        {
            var fixture = new Fixture();
            var name = fixture.Create<string>();
            var users = fixture.CreateMany<UserDto>().ToList();
            var controller = CreateControllerReturn(s =>
                    s.GetPageByNameAsync(name, lastId, It.IsAny<int>()),
                    users);

            var rv = await controller.GetPageByNameAsync(name, lastId, pageSize);
            var res = rv.Result as OkObjectResult;

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Value, Is.EquivalentTo(users));
        }


        [Test]
        public async Task DeleteByIdAsync_Success()
        {
            var fixture = new Fixture();
            var id = fixture.Create<int>();
            var user = fixture.Create<UserDetailsDto>();
            var controller = CreateControllerReturn(s =>
                    s.DeleteByIdAsync(id), user);

            var rv = await controller.DeleteByIdAsync(id);
            var res = rv.Result as OkObjectResult;

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Value, Is.EqualTo(user));
        }



        [Test]
        public async Task DeleteByIdAsync_NotFound_Throws()
        {
            var fixture = new Fixture();
            var id = fixture.Create<int>();
            var controller = CreateControllerThrows(s =>
                    s.DeleteByIdAsync(id), new UserNotFoundException());

            Assert.ThrowsAsync<UserNotFoundException>(async () =>
                    await controller.DeleteByIdAsync(id));
        }

        [Test]
        public async Task DeleteCurrentUser_Success()
        {
            var fixture = new Fixture();
            var id = fixture.Create<int>();
            var user = fixture.Create<UserDetailsDto>();
            var controller = CreateControllerWithContext(s =>
                    s.DeleteCurrentUserAsync(), user, id);

            var rv = await controller.DeleteCurrentUser();
            var res = rv.Result as OkObjectResult;

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Value, Is.EqualTo(user));
        }


        [Test]
        public async Task GetCurretUserDetailsAsync_Succcess()
        {
            var fixture = new Fixture();
            var id = fixture.Create<int>();
            var user = fixture.Create<UserPrivateDto>();
            var controller = CreateControllerWithContext(s =>
                    s.GetCurrentUserPrivateDtoAsync(), user, id);

            var rv = await controller.GetCurrentUserPrivateDto();
            var res = rv.Result as OkObjectResult;

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Value, Is.EqualTo(user));
        }
    }
}
