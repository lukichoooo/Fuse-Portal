using Core.Dtos;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Services;

namespace InfrastructureTests
{
    [TestFixture]
    public class UniversityMapperTests
    {
        private readonly IUniversityMapper _mapper = new UniversityMapper();

        private void AssertCommonPropsByName<A, B>(A a, B b)
        {
            var propsA = typeof(A).GetProperties()
                .Select(p => p.Name);
            var propsB = typeof(B).GetProperties()
                .Select(p => p.Name);
            foreach (var propName in propsA.Intersect(propsB))
            {
                var valA = typeof(A).GetProperty(propName)!.GetValue(a);
                var valB = typeof(B).GetProperty(propName)!.GetValue(b);
                Assert.That(valA, Is.EqualTo(valB));
            }
        }

        [Test]
        public void ToDto_From_University()
        {
            University uni = new()
            {
                Id = 1,
                Name = "Uni",
            };

            var res = _mapper.ToDto(uni);

            Assert.That(res, Is.Not.Null);
            AssertCommonPropsByName(res, uni);
        }


        [Test]
        public void ToDtoWithUsers_From_University()
        {
            University uni = new()
            {
                Id = 1,
                Name = "Uni",
                Users = [..
                    new[] {
                    new User() { Id = 1},
                    new User() { Id = 2},
                } ]
            };

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
            UniDto dto = new(
                    Id: 1,
                    Name: "Uni"
                    );

            var res = _mapper.ToUniversity(dto);

            Assert.That(res, Is.Not.Null);
            AssertCommonPropsByName(res, dto);
        }

        [Test]
        public void ToUniversity_From_DtoWithUsers()
        {
            UniDtoWIthUsers dto = new(
                    Id: 1,
                    Name: "Uni",
                    Users: Enumerable.Range(1, 5).Select(x =>
                        new UserDto(x, "name")).ToList()
                    );

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
