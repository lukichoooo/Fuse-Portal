using AutoFixture;
using Core.Entities;
using Core.Entities.JoinTables;
using Core.Exceptions;
using Core.Interfaces.UserUniversityTable;
using Infrastructure.Contexts;
using Infrastructure.Repos;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureTests.UserUniversityTests
{
    public class UserUniversityRepoTests
    {
        private IUserUniversityRepo _sut;
        private MyContext _context;

        [SetUp]
        public void BeforeAll()
        {
            var options = new DbContextOptionsBuilder<MyContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new MyContext(options);
            _sut = new UserUniversityRepo(_context);
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

            var res = await _sut.GetUnisPageForUserIdAsync(userId, lastId, pageSize);

            Assert.That(res, Is.Not.Null);
            Assert.That(res, Is.EquivalentTo(unis));
        }


        [TestCase(0)]
        [TestCase(22)]
        [TestCase(16)]
        public async Task GetUnisPageForUserIdAsync_PagingTest(int n)
        {
            const int pageSize = 16;
            int? lastId = null;
            const int userId = 10;


            var fixture = new Fixture();
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var user = fixture.Create<User>();
            user.Id = userId;
            user.UserUniversities = [];

            var unis = Enumerable.Range(0, n)
                .ToList()
                .ConvertAll(_ =>
                {
                    var uni = fixture.Create<University>();
                    uni.UserUniversities = [];
                    uni.UserUniversities.Add(new UserUniversity
                    {
                        UniversityId = uni.Id,
                        UserId = userId
                    }); return uni;
                });

            await _context.Users.AddAsync(user);
            foreach (var uni in unis)
                await _context.UserUniversities.AddRangeAsync(uni.UserUniversities);
            await _context.Universities.AddRangeAsync(unis);
            await _context.SaveChangesAsync();

            HashSet<int> seenId = [];
            while (true)
            {
                var res = await _sut.GetUnisPageForUserIdAsync(
                    userId, lastId, pageSize);

                Assert.That(res, Is.Not.Null);
                Assert.That(res.Count, Is.LessThanOrEqualTo(pageSize));

                if (res.Count == 0)
                    break;

                foreach (var uni in res)
                {
                    Assert.That(seenId.Contains(uni.Id), Is.False);
                    seenId.Add(uni.Id);
                }

                lastId = res[^1].Id;
            }
            Assert.That(seenId, Has.Count.EqualTo(n));
        }


        [TestCase(0)]
        [TestCase(3)]
        public async Task GetUsersByUniversityIdAsync_Success(int repeatCount)
        {
            const int pageSize = 16;
            int? lastId = null;
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


            var res = await _sut.GetUsersByUniIdPageAsync(uniId, lastId, pageSize);

            Assert.That(res, Is.Not.Null);
            Assert.That(
                    res.Select(u => u.Name).Order(),
                    Is.EquivalentTo(users.Select(u => u.Name).Order())
                    );
        }


        [TestCase(0)]
        [TestCase(22)]
        [TestCase(16)]
        public async Task GetUsersByUniIdPageAsync_PagingTest(int n)
        {
            const int pageSize = 16;
            int? lastId = null;
            const int uniId = 10;


            var fixture = new Fixture();
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var uni = fixture.Create<University>();
            uni.UserUniversities = [];
            uni.Id = uniId;

            var users = Enumerable.Range(0, n)
                .ToList()
                .ConvertAll(_ =>
                {
                    var user = fixture.Create<User>();
                    user.UserUniversities = [];
                    user.UserUniversities.Add(new UserUniversity
                    {
                        UniversityId = uniId,
                        UserId = user.Id
                    });
                    return user;
                });

            await _context.Universities.AddAsync(uni);
            foreach (var u in users)
                await _context.UserUniversities.AddRangeAsync(u.UserUniversities);
            await _context.Users.AddRangeAsync(users);
            await _context.SaveChangesAsync();

            HashSet<int> seenId = [];
            while (true)
            {
                var res = await _sut.GetUsersByUniIdPageAsync(
                    uniId, lastId, pageSize);

                Assert.That(res, Is.Not.Null);
                Assert.That(res.Count, Is.LessThanOrEqualTo(pageSize));

                if (res.Count == 0)
                    break;

                foreach (var u in res)
                {
                    Assert.That(seenId.Contains(u.Id), Is.False);
                    seenId.Add(u.Id);
                }

                lastId = res[^1].Id;
            }
            Assert.That(seenId, Has.Count.EqualTo(n));
        }




        [Test]
        public async Task AddUserToUniversityAsync_Success()
        {
            const int userId = 5, uniId = 10;
            var uu = new UserUniversity { UniversityId = uniId, UserId = userId };

            var rv = await _sut.AddUserToUniversityAsync(uu);
            var res = await _context.UserUniversities
                .FirstOrDefaultAsync(uu => uu.UserId == userId
                        && uu.UniversityId == uniId);

            Assert.That(res, Is.Not.Null);
            Assert.That(rv, Is.Not.Null);
            Assert.That(rv, Is.EqualTo(uu));
            Assert.That(res, Is.EqualTo(uu));
        }


        [Test]
        public async Task RemoveUserFromUniversityAsync_Success()
        {
            const int userId = 5, uniId = 10;
            var uu = new UserUniversity { UniversityId = uniId, UserId = userId };
            await _context.UserUniversities.AddAsync(uu);
            await _context.SaveChangesAsync();

            var rv = await _sut.RemoveUserFromUniversityAsync(userId, uniId);
            var res = await _context.UserUniversities
                .FirstOrDefaultAsync(uu => uu.UserId == userId
                        && uu.UniversityId == uniId);

            Assert.That(res, Is.Null);
            Assert.That(rv, Is.Not.Null);
            Assert.That(rv.UserId, Is.EqualTo(userId));
            Assert.That(rv.UniversityId, Is.EqualTo(uniId));
        }


        [Test]
        public async Task RemoveUserFromUniversityAsync_NotFound_Throws()
        {
            const int userId = 5, uniId = 10;
            Assert.ThrowsAsync<UserUniversityNotFoundException>(async () =>
                    await _sut.RemoveUserFromUniversityAsync(userId, uniId));
        }
    }
}
