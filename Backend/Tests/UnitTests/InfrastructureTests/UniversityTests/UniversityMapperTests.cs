using AutoFixture;
using Core.Dtos;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Services;
using UnitTests;

namespace InfrastructureTests
{
    [TestFixture]
    public class UniversityMapperTests
    {
        private readonly IUniversityMapper _mapper = new UniversityMapper();
        private readonly Fixture _fixture = new();


        [Test]
        public void ToDto_From_University()
        {
            var uni = HelperAutoFactory.CreateUniversity();

            var res = _mapper.ToDto(uni);

            Assert.That(res, Is.Not.Null);
            MapperTestHelper.AssertCommonPropsByName(res, uni);
        }


        [Test]
        public void ToDtoWithUsers_From_University()
        {
            var uni = HelperAutoFactory.CreateUniversity();

            var res = _mapper.ToDtoWithUsers(uni);

            Assert.That(res, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(res.Id, Is.EqualTo(uni.Id));
                Assert.That(res.Name, Is.EqualTo(uni.Name));
                Assert.That(res.Users.Select(u => u.Id).Order(),
                        Is.EquivalentTo(uni.Users.Select(u => u.Id).Order()));
            });
        }

        [Test]
        public void ToUniversity_From_Dto()
        {
            var dto = _fixture.Create<UniDto>();

            var res = _mapper.ToUniversity(dto);

            Assert.That(res, Is.Not.Null);
            MapperTestHelper.AssertCommonPropsByName(res, dto);
        }

        [Test]
        public void ToUniversity_From_DtoWithUsers()
        {
            UniDtoWithUsers dto = _fixture.Build<UniDtoWithUsers>()
                .With(d => d.Users, _fixture.CreateMany<UserDto>().ToList())
                .Create();

            var res = _mapper.ToUniversity(dto);

            Assert.That(res, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(res.Id, Is.EqualTo(dto.Id));
                Assert.That(res.Name, Is.EqualTo(dto.Name));
                Assert.That(res.Users.Select(u => u.Id).Order(),
                        Is.EquivalentTo(dto.Users.Select(u => u.Id).Order()));
            });
        }
    }
}
