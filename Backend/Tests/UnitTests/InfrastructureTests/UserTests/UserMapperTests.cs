using AutoFixture;
using Core.Dtos;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Services;

namespace InfrastructureTests
{
    [TestFixture]
    public class UserMapperTests
    {
        private readonly IUserMapper _mapper = new UserMapper();
        private readonly Fixture _fixture = new();

        [OneTimeSetUp]
        public void BeforeAll()
        { }
        [Test]
        public void ToDto_From_User()
        {
            var user = _fixture.Build<User>()
                .With(u => u.Universities, [])
                .With(u => u.Faculties, [])
                .Create();

            var res = _mapper.ToDto(user);

            Assert.That(res, Is.Not.Null);
            MapperTestHelper.AssertCommonPropsByName(user, res);
        }


        [Test]
        public void ToRequestDto_From_User()
        {
            var user = _fixture.Build<User>()
                .With(u => u.Universities, [])
                .With(u => u.Faculties, [])
                .Create();

            var res = _mapper.ToPrivateDto(user);


            Assert.That(res, Is.Not.Null);
            MapperTestHelper.AssertCommonPropsByName(res, user);
        }


        [Test]
        public void ToDetailsDto_From_User()
        {
            var user = _fixture.Build<User>()
                .With(u => u.Universities, _fixture.Build<University>().OmitAutoProperties().CreateMany().ToList())
                .With(u => u.Faculties, _fixture.Build<Faculty>().OmitAutoProperties().CreateMany().ToList())
                .With(u => u.Address, _fixture.Create<Address>())
                .Create();

            var res = _mapper.ToDetailsDto(user);


            Assert.That(res, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(res.Id, Is.EqualTo(user.Id));
                Assert.That(res.Name, Is.EqualTo(user.Name));
                Assert.That(res.Universities.Select(u => u.Id).Order(),
                        Is.EquivalentTo(user.Universities.Select(u => u.Id).Order()));
                Assert.That(res.Faculties.Order(),
                        Is.EquivalentTo(user.Faculties.Select(u => u.Name).Order()));
            });
        }
    }
}
