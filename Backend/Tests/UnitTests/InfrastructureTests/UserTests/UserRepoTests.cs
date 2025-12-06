using AutoFixture;
using Core.Entities;
using Core.Entities.Portal;
using Core.Exceptions;
using Core.Interfaces;
using Infrastructure.Contexts;
using Infrastructure.Repos;
using Microsoft.EntityFrameworkCore;
using UnitTests;

namespace InfrastructureTests.UserTests
{
    [TestFixture]
    public class UserRepoTests
    {
        private IUserRepo _repo;
        private MyContext _context;
        private static readonly Fixture _globalFixture = new();

        [SetUp]
        public void BeforeAll()
        {
            var options = new DbContextOptionsBuilder<MyContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // random Id
                .Options;
            _context = new MyContext(options);
            _repo = new UserRepo(_context);
        }

        [TearDown]
        public async Task AfterAllAsync()
        {
            await _context.DisposeAsync();
        }



        [TestCase(new int[] { })]
        [TestCase(new[] { 1, 2, 5, 6, 7 })]
        public async Task GetAllAsync_Success(int[] ids)
        {
            const int pageSize = 16;
            var users = ids.Select(id => HelperAutoFactory.CreateUser(id)).ToList();
            await _context.AddRangeAsync(users);
            await _context.SaveChangesAsync();

            var res = await _repo.GetAllPageAsync(int.MinValue, pageSize);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Select(u => u.Name).Order(),
                    Is.EquivalentTo(users.Select(u => u.Name).Order()));
        }


        [Test]
        public async Task GetAllAsync_Paged_Success()
        {
            const int pageSize = 16;
            var users = Enumerable.Range(1, 25)
                    .Reverse()
                    .Select(id => HelperAutoFactory.CreateUser(id))
                    .ToList();
            await _context.AddRangeAsync(users);
            await _context.SaveChangesAsync();

            var res = await _repo.GetAllPageAsync(-1, pageSize);

            Assert.That(res, Is.Not.Null);
            Assert.That(res,
                    Is.EquivalentTo(users.OrderBy(u => u.Id)
                        .Take(pageSize)));
        }

        [TestCase(5)]
        [TestCase(20)]
        public async Task GetAllAsync_Paged_Success_2(int lastId)
        {
            const int pageSize = 16;
            var users = Enumerable.Range(1, 25)
                    .Reverse()
                    .Select(id => HelperAutoFactory.CreateUser(id))
                    .ToList();
            await _context.AddRangeAsync(users);
            await _context.SaveChangesAsync();

            var res = await _repo.GetAllPageAsync(lastId, pageSize);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Min(u => u.Id), Is.EqualTo(lastId + 1));
        }


        [Test]
        public async Task SearchByName_Paged_Success()
        {
            const string name = "luka";
            const int pageSize = 16;
            var users = Enumerable.Range(1, 25)
                .Reverse()
                .Select(id =>
                {
                    var user = HelperAutoFactory.CreateUser(id);
                    user.Name = name.ToUpper() + id.ToString();
                    return user;
                })
                .ToList();
            await _context.AddRangeAsync(users);
            await _context.SaveChangesAsync();

            var res = await _repo.PageByNameAsync(name, -1, pageSize);

            Assert.That(res, Is.Not.Null);
            Assert.That(res,
                    Is.EquivalentTo(users
                        .OrderBy(u => u.Id)
                        .Take(pageSize)));
        }



        [Test]
        public async Task GetByIdAsync_Found()
        {
            const int id = 5;
            var user = HelperAutoFactory.CreateUser(id);
            await _context.AddAsync(user);
            await _context.SaveChangesAsync();

            var res = await _repo.GetByIdAsync(id);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Id, Is.EqualTo(id));
        }



        [Test]
        public async Task GetByIdAsync_NotThere_Null()
        {
            const int id = 5;
            var res = await _repo.GetByIdAsync(id);
            Assert.That(res, Is.Null);
        }


        [Test]
        public async Task GetByEmail_Found()
        {
            const string email = "myemial@explam.co";
            var user = HelperAutoFactory.CreateUser();
            user.Email = email;
            await _context.AddAsync(user);
            await _context.SaveChangesAsync();

            var res = await _repo.GetByEmailAsync(email);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Email, Is.EqualTo(email));
        }


        [Test]
        public async Task GetByEmail_NotThere_Null()
        {
            const string email = "myemial@explam.co";
            var res = await _repo.GetByEmailAsync(email);
            Assert.That(res, Is.Null);
        }

        [Test]
        public async Task CreateAsync_Success()
        {
            const int id = 2;
            var user = HelperAutoFactory.CreateUser(id);

            var rv = await _repo.CreateAsync(user);
            var res = await _context.Users.FindAsync(id);

            Assert.That(rv, Is.Not.Null);
            Assert.That(res, Is.Not.Null);
            Assert.That(res, Is.EqualTo(user));
        }


        [Test]
        public async Task UpdateUserCredentialsAsync_Success()
        {
            const int id = 2;
            var user = HelperAutoFactory.CreateUser(id);
            user.Email = "oldEmail@gmail.com";
            user.Name = "wOldName";
            var newUser = HelperAutoFactory.CreateUser(id);
            user.Email = "newEmail@gmai.com";
            user.Name = "NewNAme";
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var rv = await _repo.UpdateUserCredentialsAsync(newUser);
            var res = await _context.Users.FindAsync(id);

            Assert.That(res, Is.Not.Null);
            Assert.That(rv, Is.Not.Null);
            Assert.That(rv, Is.EqualTo(res));
            Assert.That(rv, Is.EqualTo(user));
            Assert.That(user.Email, Is.EqualTo(newUser.Email));
            Assert.That(user.Name, Is.EqualTo(newUser.Name));
        }

        [Test]
        public async Task UpdateUserCredentialsAsync_NotFound_Throws()
        {
            const int id = 2;
            var newUser = HelperAutoFactory.CreateUser(id);
            Assert.ThrowsAsync<UserNotFoundException>(async () =>
                    await _repo.UpdateUserCredentialsAsync(newUser));
        }


        [TestCase(0)]
        [TestCase(4)]
        public async Task GetUserDetailsByIdAsync_Success(int count)
        {
            const int id = 5;

            var fixture = new Fixture() { RepeatCount = count };
            fixture.Behaviors.Add(new OmitOnRecursionBehavior(1));

            var unis = fixture.CreateMany<University>().ToList();
            var courses = fixture.CreateMany<Course>().ToList();
            var subjectsEnroll = fixture.CreateMany<Subject>().ToList();
            var subjectsTeaching = fixture.CreateMany<Subject>().ToList();
            var user = fixture.Create<User>();

            user.Id = id;
            user.UserUniversities = unis
                .ConvertAll(uni =>
                    new UserUniversity
                    {
                        University = uni,
                        User = user,
                    });
            user.Courses = courses;
            user.SubjectEnrollments = subjectsEnroll;
            user.TeachingSubjects = subjectsTeaching;

            await _context.Universities.AddRangeAsync(unis);
            await _context.Subjects.AddRangeAsync(subjectsEnroll);
            await _context.Subjects.AddRangeAsync(subjectsTeaching);
            await _context.Courses.AddRangeAsync(courses);
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var res = await _repo.GetUserDetailsByIdAsync(id);

            Assert.That(res, Is.Not.Null);
            Assert.That(res, Is.EqualTo(user));
            Assert.Multiple(() =>
            {
                Assert.That(
                    res.Courses.ConvertAll(f => f.Name).Order(),
                    Is.EqualTo(courses.ConvertAll(f => f.Name).Order())
                );

                Assert.That(
                    res.UserUniversities.ConvertAll(uu => uu.University.Name).Order(),
                    Is.EquivalentTo(unis.ConvertAll(u => u.Name).Order())
                );
            });
        }

    }
}
