using AutoFixture;
using Core.Entities;
using Core.Interfaces.UserUniversityTable;
using Infrastructure.Contexts;
using Infrastructure.Repos;
using Microsoft.EntityFrameworkCore;

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
            const int userId = 5;

            var fixture = new Fixture() { RepeatCount = repeatCount };
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var unis = fixture.Build<University>()
                .CreateMany()
                .ToList();
            unis.ConvertAll(u => u.UserUniversities = []);
            var user = fixture.Build<User>()
                .With(u => u.Id, userId)
                .Create();
            user.UserUniversities = [];

            user.UserUniversities = unis
                .ConvertAll(uni => new UserUniversity
                {
                    UserId = userId,
                    UniversityId = uni.Id
                });

            await _context.Universities.AddRangeAsync(unis);
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var res = await _repo.GetUnisPageForUserIdAsync(userId, lastId, pageSize);

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
                .CreateMany()
                .ToList();
            users.ConvertAll(u => u.UserUniversities = []);

            var uni = fixture.Build<University>()
                .Create();
            uni.UserUniversities = [];
            uni.Id = uniId;

            uni.UserUniversities = users.ConvertAll(u => new UserUniversity
            {
                UserId = u.Id,
                UniversityId = uni.Id,
            });

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
