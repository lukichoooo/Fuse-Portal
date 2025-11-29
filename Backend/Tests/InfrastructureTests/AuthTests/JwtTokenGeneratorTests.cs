using System.IdentityModel.Tokens.Jwt;
using AutoFixture;
using Core.Dtos;
using Core.Entities;
using Core.Interfaces;
using Core.Settings;
using Infrastructure.Services;
using Microsoft.Extensions.Options;

namespace Tests.Infrastructure
{
    [TestFixture]
    public class JwtTokenGeneratorTests
    {
        private IJwtTokenGenerator _jwt;
        private readonly JwtSettings _settings = new()
        {
            Key = "asdadadaisodASifimallivewhyareumyremedyosajfa",
            Issuer = "lukaco",
            Audience = "tqven",
            AccessTokenExpiration = 5,
            RefreshTokenExpiration = 30
        };

        [OneTimeSetUp]
        public void BeforeAll()
        {
            var options = Options.Create(_settings);
            _jwt = new JwtTokenGenerator(options);
        }

        [Test]
        public void GenerateToken_Returns_Valid_Result()
        {
            var fixture = new Fixture();
            var user = fixture.Build<User>()
                .With(u => u.Universities, [])
                .With(u => u.Faculties, [])
                .Create();
            var handler = new JwtSecurityTokenHandler();

            var tokenString = _jwt.GenerateToken(user);
            var token = handler.ReadJwtToken(tokenString);
            var claims = token.Claims;

            var id = int.Parse(claims.FirstOrDefault(c => c.Type == "id")!.Value);
            var email = claims.FirstOrDefault(c => c.Type == "email")!.Value;
            var role = claims.FirstOrDefault(c => c.Type == "role")!.Value;

            Assert.That(id, Is.EqualTo(user.Id));
            Assert.That(email, Is.EqualTo(user.Email));
            Assert.That(role, Is.EqualTo(user.Role.ToString()));
        }
    }
}
