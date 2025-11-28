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
        public void ToDto_From_User()
        {
            User user = new()
            {
                Id = 1,
                Name = "namee",
            };

            var res = _mapper.ToDto(user);

            Assert.That(res, Is.Not.Null);
            AssertCommonPropsByName(user, res);
        }


        [Test]
        public void ToPrivateInfo_From_User()
        {
            User user = new()
            {
                Id = 1,
                Name = "namee",
                Email = "exampl@email.cm",
                Password = "myPAss222"
            };

            var res = _mapper.ToPrivateInfo(user);


            Assert.That(res, Is.Not.Null);
            AssertCommonPropsByName(res, user);
        }


        [Test]
        public void ToDetailsDto_From_User()
        {
            User user = new()
            {
                Id = 1,
                Name = "namee",
                Universities = Enumerable.Range(1, 5)
                    .Select(x => new University { Id = x })
                    .ToList(),
                Faculties = Enumerable.Range(0, 26)
                    .Select(i => new Faculty { Name = ((char)('a' + i)).ToString() })
                    .ToList()
            };

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
