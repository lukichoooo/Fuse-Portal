using System.Linq.Expressions;
using System.Security.Claims;
using AutoFixture;
using Core.Dtos;
using Core.Exceptions;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presentation.Controllers;

namespace PresentationTests
{
    [TestFixture]
    public class UsercontrollerTests
    {
        private static UserController CreateControllerReturn<T>(
                Expression<Func<IUserService, Task<T>>> exp,
                T returnValue)
        {
            var mock = new Mock<IUserService>();
            mock.Setup(exp).ReturnsAsync(returnValue);
            return new UserController(mock.Object);
        }

        private static UserController CreateControllerThrows<T>(
                Expression<Func<IUserService, Task<T>>> exp,
                Exception e)
        {
            var mock = new Mock<IUserService>();
            mock.Setup(exp).ThrowsAsync(e);
            return new UserController(mock.Object);
        }


        private UserController CreateControllerWithContext<T>(
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

            return new UserController(mock.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = httpContext }
            };
        }


        [Test]
        public async Task GetAllPageAsync_Success()
        {
            var fixture = new Fixture();
            var users = fixture.CreateMany<UserDto>().ToList();
            var controller = CreateControllerReturn(s =>
                    s.GetAllPageAsync(It.IsAny<int>(), It.IsAny<int>()),
                    users);

            var res = await controller.GetAllPageAsync() as OkObjectResult;

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Value, Is.EqualTo(users));
        }


        [Test]
        public async Task GetById_Success()
        {
            var fixture = new Fixture();
            var user = fixture.Create<UserDto>();
            var controller = CreateControllerReturn(s =>
                    s.GetByIdAsync(It.IsAny<int>()), user);

            var res = await controller.GetByIdAsync(user.Id) as OkObjectResult;

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



        [Test]
        public async Task PageByName_Success()
        {
            var fixture = new Fixture();
            var name = fixture.Create<string>();
            var users = fixture.CreateMany<UserDto>().ToList();
            var controller = CreateControllerReturn(s =>
                    s.GetPageByNameAsync(name, It.IsAny<int>(), It.IsAny<int>()),
                    users);

            var res = await controller.GetPageByNameAsync(name) as OkObjectResult;

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Value, Is.EqualTo(users));
        }


        [Test]
        public async Task DeleteByIdAsync_Success()
        {
            var fixture = new Fixture();
            var id = fixture.Create<int>();
            var user = fixture.Create<UserPrivateDto>();
            var controller = CreateControllerReturn(s =>
                    s.DeleteByIdAsync(id), user);

            var res = await controller.DeleteByIdAsync(id) as OkObjectResult;

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Value, Is.EqualTo(user));
        }



        [Test]
        public async Task DeleteByIdAsync_NotFound_Throws()
        {
            var fixture = new Fixture();
            var id = fixture.Create<int>();
            var user = fixture.Create<UserPrivateDto>();
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
            var user = fixture.Create<UserPrivateDto>();
            var controller = CreateControllerWithContext(s =>
                    s.DeleteByIdAsync(id), user, id);

            var res = await controller.DeleteCurrentUser() as OkObjectResult;

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
                    s.GetPrivateDtoById(id), user, id);

            var res = await controller.GetCurrentUserPrivateDto() as OkObjectResult;

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Value, Is.EqualTo(user));
        }
    }
}
