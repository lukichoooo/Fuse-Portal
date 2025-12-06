using AutoFixture;
using Core.Interfaces;
using Infrastructure.Services;
using UnitTests;

namespace InfrastructureTests
{
    [TestFixture]
    public class UniversityMapperTests
    {
        private readonly IUniversityMapper _mapper = new UniversityMapper();

        [Test]
        public void ToDto_From_University()
        {
            var uni = HelperAutoFactory.CreateUniversity();

            var res = _mapper.ToDto(uni);

            Assert.That(res, Is.Not.Null);
            HelperMapperTest.AssertCommonPropsByName(res, uni);
        }


        [Test]
        public void ToDtoWithUsers_From_University()
        {
            var uni = HelperAutoFactory.CreateUniversity();

            var res = _mapper.ToDtoWithUsers(uni);

            Assert.That(res, Is.Not.Null);
            HelperMapperTest.AssertCommonPropsByName(res, uni);
            Assert.Multiple(() =>
            {
                Assert.That(res.Id, Is.EqualTo(uni.Id));
                Assert.That(res.Name, Is.EqualTo(uni.Name));
                Assert.That(res.Users.Select(u => u.Id).Order(),
                        Is.EquivalentTo(uni.UserUniversities.Select(uu => uu.UserId).Order()));
            });
        }
    }
}
