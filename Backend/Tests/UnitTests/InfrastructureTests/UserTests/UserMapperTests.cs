using Core.Interfaces;
using Infrastructure.Services;
using Infrastructure.Services.Portal;
using UnitTests;

namespace InfrastructureTests
{
    [TestFixture]
    public class UserMapperTests
    {
        private readonly IUserMapper _mapper = new UserMapper(
                new PortalMapper(),
                new UniversityMapper());

        [OneTimeSetUp]
        public void BeforeAll() { }

        [Test]
        public void ToDto_From_User()
        {
            var user = HelperAutoFactory.CreateUser();

            var res = _mapper.ToDto(user);

            Assert.That(res, Is.Not.Null);
            HelperMapperTest.AssertCommonPropsByName(user, res);
        }



        [Test]
        public void ToRequestDto_From_User()
        {
            var user = HelperAutoFactory.CreateUser();

            var res = _mapper.ToPrivateDto(user);

            Assert.That(res, Is.Not.Null);
            HelperMapperTest.AssertCommonPropsByName(res, user);
        }


        [Test]
        public void ToDetailsDto_From_User()
        {
            var user = HelperAutoFactory.CreateUser();

            var res = _mapper.ToDetailsDto(user);


            Assert.That(res, Is.Not.Null);
            HelperMapperTest.AssertCommonPropsByName(res, user);
        }
    }
}
