using AutoFixture;
using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;
using Infrastructure.Contexts;
using Infrastructure.Repos;
using Microsoft.EntityFrameworkCore;

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


        private static User CreateUserById(int id)
            => _globalFixture.Build<User>()
            .With(u => u.Universities, [])
            .With(u => u.Faculties, [])
            .With(u => u.Id, id)
            .Create();

        private static University CreateUniById(int id)
            => _globalFixture.Build<University>()
                .With(uni => uni.Id, id)
                .With(uni => uni.Users, [])
                .With(uni => uni.Address, _globalFixture.Create<Address>())
                .Create();

        [TestCase(new int[] { })]
        [TestCase(new[] { 1, 2, 5, 6, 7 })]
        public async Task GetAllAsync_Success(int[] ids)
        {
            const int pageSize = 16;
            var users = ids.Select(id => CreateUserById(id)).ToList();
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
                    .Select(id => CreateUserById(id))
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
                    .Select(id => CreateUserById(id))
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
                    var user = CreateUserById(id);
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
            var user = CreateUserById(id);
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
            var user = CreateUserById(0);
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
            var user = CreateUserById(id);

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
            var user = CreateUserById(id);
            user.Email = "oldEmail@gmail.com";
            var newUser = CreateUserById(id);
            user.Email = "newEmail@gmai.com";
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var rv = await _repo.UpdateUserCredentialsAsync(newUser);
            var res = await _context.Users.FindAsync(id);

            Assert.That(res, Is.Not.Null);
            Assert.That(rv, Is.Not.Null);
            Assert.That(rv, Is.EqualTo(res));
            Assert.That(rv, Is.EqualTo(user));
            Assert.That(user.Email, Is.EqualTo(newUser.Email));
        }

        [Test]
        public async Task UpdateUserCredentialsAsync_NotFound_Throws()
        {
            const int id = 2;
            var newUser = CreateUserById(id);
            Assert.ThrowsAsync<UserNotFoundException>(async () =>
                    await _repo.UpdateUserCredentialsAsync(newUser));
        }

        [TestCase(new int[] { })]
        [TestCase(new[] { 1, 4, 32 })]
        public async Task GetUniversitiesForUserAsync_Success(int[] uniIds)
        {
            const int id = 5;
            var fixture = new Fixture();
            var unis = uniIds
                .Select(id => CreateUniById(id))
                .ToList();
            var user = fixture.Build<User>()
                .With(u => u.Universities, unis)
                .With(u => u.Faculties, [])
                .With(u => u.Id, id)
                .Create();
            await _context.Universities.AddRangeAsync(unis);
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var res = await _repo.GetUnisForUserAsync(id);

            Assert.That(res, Is.Not.Null);
            Assert.That(res, Is.EquivalentTo(unis));
        }

        [Test]
        public async Task GetUniversitiesForUserAsync_NotFound_Throws()
        {
            const int id = 5;
            Assert.ThrowsAsync<UserNotFoundException>(async () =>
                    await _repo.GetUnisForUserAsync(id));
        }

        [TestCase(new int[] { })]
        [TestCase(new[] { 1, 4, 32 })]
        public async Task GetFullById_Success(int[] ids)
        {
            const int id = 5;

            var unis = ids
                .Select(id =>
                _globalFixture.Build<University>()
                    .With(uni => uni.Id, id)
                    .With(uni => uni.Users, [])
                    .With(uni => uni.Address, _globalFixture.Create<Address>())
                    .Create())
                .ToList();
            var faculties = ids
                .Select(id => _globalFixture.Build<Faculty>()
                        .With(f => f.Users, [])
                        .With(f => f.Id, id)
                        .Create())
                .ToList();
            var user = _globalFixture.Build<User>()
                .With(u => u.Id, id)
                .With(u => u.Universities, unis)
                .With(u => u.Faculties, faculties)
                .Create();

            await _context.Universities.AddRangeAsync(unis);
            await _context.Faculties.AddRangeAsync(faculties);
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var res = await _repo.GetUserDetailsAsync(id);

            Assert.That(res, Is.Not.Null);
            Assert.That(res, Is.EqualTo(user));
            Assert.Multiple(() =>
            {
                Assert.That(res.Faculties.Select(f => f.Name),
                                Is.EqualTo(faculties.Select(f => f.Name)));
                Assert.That(res.Universities.Select(u => u.Id),
                        Is.EqualTo(unis.Select(u => u.Id)));
            });

        }


        [Test]
        public async Task GetFullById_NotFOund_Throws()
        {
            const int id = 5;
            Assert.ThrowsAsync<UserNotFoundException>(async () =>
                    await _repo.GetUserDetailsAsync(id));
        }

    }
}
