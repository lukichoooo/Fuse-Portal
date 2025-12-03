using System.Linq.Expressions;
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
    public class UniversityControllerTests
    {
        private static UniversityController CreateControllerReturn<T>(
                Expression<Func<IUniversityService, Task<T>>> exp,
                T returnValue)
        {
            var mock = new Mock<IUniversityService>();
            mock.Setup(exp).ReturnsAsync(returnValue);
            return new UniversityController(mock.Object);
        }

        private static UniversityController CreateControllerThrows<T>(
                Expression<Func<IUniversityService, Task<T>>> exp,
                Exception e)
        {
            var mock = new Mock<IUniversityService>();
            mock.Setup(exp).ThrowsAsync(e);
            return new UniversityController(mock.Object);
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


        [Test]
        public async Task GetPageByNameLikeAsync_Success()
        {
            var fixture = new Fixture();
            var name = fixture.Create<string>();
            var unis = fixture.CreateMany<UniDto>().ToList();
            var controller = CreateControllerReturn(
                    s => s.GetPageByNameLikeAsync(name, It.IsAny<int>(), It.IsAny<int>()),
                        unis);

            var rv = await controller.GetPageByNameLikeAsync(name);
            var res = rv.Result as OkObjectResult;

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Value, Is.EqualTo(unis));
        }




        [Test]
        public async Task GetUsersPageAsync_Success()
        {
            var fixture = new Fixture();
            var uniId = fixture.Create<int>();
            var users = fixture.CreateMany<UserDto>().ToList();
            var controller = CreateControllerReturn(
                    s => s.GetUsersPageAsync(uniId, It.IsAny<int>(), It.IsAny<int>()),
                        users);

            var rv = await controller.GetUsersPageAsync(uniId);
            var res = rv.Result as OkObjectResult;

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Value, Is.EqualTo(users));
        }



        [Test]
        public async Task GetUsersPageAsync_NotFound_Throws()
        {
            var fixture = new Fixture();
            var uniId = fixture.Create<int>();
            var controller = CreateControllerThrows(
                    s => s.GetUsersPageAsync(uniId, It.IsAny<int>(), It.IsAny<int>()),
                        new UniversityNotFoundException());

            Assert.ThrowsAsync<UniversityNotFoundException>(async () =>
                    await controller.GetUsersPageAsync(uniId));
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
