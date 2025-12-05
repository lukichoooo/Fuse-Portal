using System.Linq.Expressions;
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
    public class UniversityControllerTests
    {
        private static readonly ControllerSettings _settings = new()
        {
            DefaultPageSize = 16,
            SmallPageSize = 8,
            BigPageSize = 32
        };
        private static UniversityController CreateControllerReturn<T>(
                Expression<Func<IUniversityService, Task<T>>> exp,
                T returnValue)
        {
            var mock = new Mock<IUniversityService>();
            mock.Setup(exp).ReturnsAsync(returnValue);
            return new UniversityController(mock.Object, Options.Create(_settings));
        }

        private static UniversityController CreateControllerThrows<T>(
                Expression<Func<IUniversityService, Task<T>>> exp,
                Exception e)
        {
            var mock = new Mock<IUniversityService>();
            mock.Setup(exp).ThrowsAsync(e);
            return new UniversityController(mock.Object, Options.Create(_settings));
        }


        [Test]
        public async Task GetByIdAsync_Success()
        {
            var fixture = new Fixture();
            var uni = fixture.Create<UniDto>();
            var controller = CreateControllerReturn(
                    s => s.GetByIdAsync(uni.Id), uni);

            var rv = await controller.GetByIdAsync(uni.Id);
            var res = rv.Result as OkObjectResult;

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Value, Is.EqualTo(uni));
        }



        [Test]
        public async Task GetByIdAsync_NotFound_Throws()
        {
            var fixture = new Fixture();
            var id = fixture.Create<int>();
            var controller = CreateControllerThrows(
                    s => s.GetByIdAsync(id), new UniversityNotFoundException());

            Assert.ThrowsAsync<UniversityNotFoundException>(async () =>
                    await controller.GetByIdAsync(id));
        }


        [TestCase(null, null)]
        [TestCase(0, null)]
        public async Task GetPageByNameLikeAsync_Success(int? lastId, int? pageSize)
        {
            var fixture = new Fixture();
            var name = fixture.Create<string>();
            var unis = fixture.CreateMany<UniDto>().ToList();
            var controller = CreateControllerReturn(
                    s => s.GetPageByNameLikeAsync(name, lastId, It.IsAny<int>()),
                        unis);

            var rv = await controller.GetPageByNameLikeAsync(name, lastId, pageSize);
            var res = rv.Result as OkObjectResult;

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Value, Is.EqualTo(unis));
        }




        [TestCase(null, null)]
        [TestCase(0, null)]
        public async Task GetUsersPageAsync_Success(int? lastId, int? pageSize)
        {
            var fixture = new Fixture();
            var uniId = fixture.Create<int>();
            var users = fixture.CreateMany<UserDto>().ToList();
            var controller = CreateControllerReturn(
                    s => s.GetUsersPageAsync(uniId, lastId, It.IsAny<int>()),
                        users);

            var rv = await controller.GetUsersPageAsync(uniId, lastId, pageSize);
            var res = rv.Result as OkObjectResult;

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Value, Is.EqualTo(users));
        }



        [TestCase(null, null)]
        [TestCase(0, null)]
        public async Task GetUsersPageAsync_NotFound_Throws(int? lastId, int? pageSize)
        {
            var fixture = new Fixture();
            var uniId = fixture.Create<int>();
            var controller = CreateControllerThrows(
                    s => s.GetUsersPageAsync(uniId, lastId, It.IsAny<int>()),
                        new UniversityNotFoundException());

            Assert.ThrowsAsync<UniversityNotFoundException>(async () =>
                    await controller.GetUsersPageAsync(uniId, lastId, pageSize));
        }



        [Test]
        public async Task DeleteByIdAsync_Success()
        {
            var fixture = new Fixture();
            var uni = fixture.Create<UniDto>();
            var controller = CreateControllerReturn(
                    s => s.DeleteByIdAsync(uni.Id), uni);

            var rv = await controller.DeleteByIdAsync(uni.Id);
            var res = rv.Result as OkObjectResult;

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Value, Is.EqualTo(uni));
        }


        [Test]
        public async Task DeleteByIdAsync_NotFound_Throws()
        {
            var fixture = new Fixture();
            var uni = fixture.Create<UniDto>();
            var controller = CreateControllerThrows(
                    s => s.DeleteByIdAsync(uni.Id), new UniversityNotFoundException());

            Assert.ThrowsAsync<UniversityNotFoundException>(async () =>
                    await controller.DeleteByIdAsync(uni.Id));
        }


        [Test]
        public async Task UpdateAsync_Success()
        {
            var fixture = new Fixture();
            var uniRequest = fixture.Create<UniRequestDto>();
            var uniDto = fixture.Create<UniDto>();
            var controller = CreateControllerReturn(
                    s => s.UpdateAsync(uniRequest), uniDto);

            var rv = await controller.UpdateAsync(uniRequest);
            var res = rv.Result as OkObjectResult;

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Value, Is.EqualTo(uniDto));
        }


        [Test]
        public async Task UpdateAsync_NotFound_Throws()
        {
            var fixture = new Fixture();
            var uniRequest = fixture.Create<UniRequestDto>();
            var controller = CreateControllerThrows(
                    s => s.UpdateAsync(uniRequest), new UniversityNotFoundException());

            Assert.ThrowsAsync<UniversityNotFoundException>(async () =>
                    await controller.UpdateAsync(uniRequest));
        }



        [Test]
        public async Task CreateAsync_Success()
        {
            var fixture = new Fixture();
            var uniRequest = fixture.Create<UniRequestDto>();
            var uniDto = fixture.Create<UniDto>();
            var controller = CreateControllerReturn(
                    s => s.CreateAsync(uniRequest), uniDto);

            var rv = await controller.CreateAsync(uniRequest);
            var res = rv.Result as OkObjectResult;

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Value, Is.EqualTo(uniDto));
        }


        [Test]
        public async Task CreateAsync_NotFound_Throws()
        {
            var fixture = new Fixture();
            var uniRequest = fixture.Create<UniRequestDto>();
            var controller = CreateControllerThrows(
                    s => s.CreateAsync(uniRequest), new UniversityAlreadyExists());

            Assert.ThrowsAsync<UniversityAlreadyExists>(async () =>
                    await controller.CreateAsync(uniRequest));
        }
    }
}
