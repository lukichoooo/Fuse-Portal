using AutoFixture;
using Core.Dtos;
using Core.Entities;
using Core.Interfaces.Auth;
using Infrastructure.Services.Auth;

namespace InfrastructureTests.AuthTests
{
    [TestFixture]
    public class AuthMapperTests
    {
        private readonly IAuthMapper _mapper = new AuthMapper();

        [Test]
        public void ToUser_Success()
        {
            var fixture = new Fixture();
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var request = fixture.Create<RegisterRequest>();

            var res = _mapper.ToUser(request);

            Assert.That(res, Is.Not.Null);
            HelperMapperTest.AssertCommonPropsByName(res, request);
        }
    }
}
