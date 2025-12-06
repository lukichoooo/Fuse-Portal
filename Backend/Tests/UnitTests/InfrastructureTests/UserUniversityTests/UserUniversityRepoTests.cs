using AutoFixture;
using Core.Entities;
using Core.Interfaces.UserUniversity;
using Infrastructure.Contexts;
using Infrastructure.Repos;
using Microsoft.EntityFrameworkCore;
using UnitTests;

namespace InfrastructureTests.UserUniversityTests
{
    public class UserUniversityRepoTests
    {
        private IUserUniversityRepo _repo;
        private MyContext _context;

        [SetUp]
        public void BeforeAll()
        {
            var options = new DbContextOptionsBuilder<MyContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new MyContext(options);
            _repo = new UserUniversityRepo(_context);
        }

        [TearDown]
        public async Task AfterAllAsync()
        {
            await _context.DisposeAsync();
        }



        [TestCase(0)]
        [TestCase(4)]
        public async Task GetUniversitiesForUserAsync_Success(int repeatCount)
        {
            const int pageSize = 16;
            int? lastId = null;
            const int id = 5;
            var unis = HelperAutoFactory.CreateUniversityList(repeatCount);
            var user = HelperAutoFactory.CreateUser();
            user.UserUniversities = [];

            foreach (var uni in unis)
            {
                user.UserUniversities.Add(
                        new UserUniversity
                        {
                            UserId = id,
                            UniversityId = uni.Id,
                        });
            }

            user.Id = id;

            await _context.Universities.AddRangeAsync(unis);
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var res = await _repo.GetUnisPageForUserIdAsync(id, lastId, pageSize);

            Assert.That(res, Is.Not.Null);
            Assert.That(res, Is.EquivalentTo(unis));
        }




        [TestCase(0)]
        [TestCase(3)]
        public async Task GetUsersByUniversityIdAsync_Success(int repeatCount)
        {
            const int lastId = int.MinValue, pageSize = 16;
            const int uniId = 4;

            var fixture = new Fixture() { RepeatCount = repeatCount };
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var users = fixture.Build<User>()
                .With(u => u.UserUniversities, [])
                .CreateMany()
                .ToList();
            var uni = fixture.Build<University>()
                .With(uni => uni.UserUniversities, [])
                .Create();
            uni.Id = uniId;

            uni.UserUniversities = users.ConvertAll(
                        u => new UserUniversity
                        {
                            UserId = u.Id,
                            UniversityId = uniId
                        });

            await _context.AddRangeAsync(uni.UserUniversities);
            await _context.Users.AddRangeAsync(users);
            await _context.Universities.AddAsync(uni);
            await _context.SaveChangesAsync();

            var res = await _repo.GetUsersByUniIdPageAsync(uniId, lastId, pageSize);

            Assert.That(res, Is.Not.Null);
            Assert.That(
                    res.Select(u => u.Name).Order(),
                    Is.EquivalentTo(users.Select(u => u.Name).Order())
                    );
        }


    }
}
