using AutoFixture;
using Core.Dtos;
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
        public void ToUser_From_UserUpdateRequest()
        {
            var fixture = new Fixture();
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var request = fixture.Build<UserUpdateRequest>()
                .With(x => x.Courses, [])
                .With(x => x.Universities, [])
                .Create();
            var res = _mapper.ToUser(request);

            Assert.That(res, Is.Not.Null);
            HelperMapperTest.AssertCommonPropsByName(res, request);
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
