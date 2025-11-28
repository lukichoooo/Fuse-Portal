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

        [OneTimeSetUp]
        public void BeforeAll()
        {
            var options = new DbContextOptionsBuilder<MyContext>()
                .UseInMemoryDatabase("MyInMem")
                .Options;
            _context = new MyContext(options);
            _repo = new UserRepo(_context);
        }

        [OneTimeTearDown]
        public async Task AfterAllAsync()
        {
            await _context.DisposeAsync();
        }

        [SetUp]
        public async Task BeforeEach()
        {
            await _context.Database.EnsureCreatedAsync();

            _context.Users.RemoveRange(_context.Users);
            _context.Universities.RemoveRange(_context.Universities);

            await _context.SaveChangesAsync();
        }


        [TestCase(new int[] { })]
        [TestCase(new[] { 1, 2, 5, 6, 7 })]
        public async Task GetAllAsync_Success(int[] ids)
        {
            const int pageSize = 16;
            var users = ids.Select(id =>
                    new User { Id = id }).ToList();
            await _context.AddRangeAsync(users);
            await _context.SaveChangesAsync();

            var res = await _repo.GetAllPageAsync(-1, pageSize);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.OrderBy(u => u.Id),
                    Is.EquivalentTo(users.OrderBy(u => u.Id)));
        }


        [Test]
        public async Task GetAllAsync_Paged_Success()
        {
            const int pageSize = 16;
            var users = Enumerable.Range(1, 25)
                    .Reverse()
                    .Select(id =>
                    new User { Id = id }).ToList();
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
                    .Select(id => new User { Id = id })
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
            const int pageSize = 16;
            var users = Enumerable.Range(1, 25)
                .Reverse()
                .Select(id => new User { Id = id, Name = "lukA" })
                .ToList();
            await _context.AddRangeAsync(users);
            await _context.SaveChangesAsync();

            var res = await _repo.PageByNameAsync("luka", -1, pageSize);

            Assert.That(res, Is.Not.Null);
            Assert.That(res,
                    Is.EquivalentTo(users.OrderBy(u => u.Id)
                        .Take(pageSize)));
        }



        [Test]
        public async Task GetByIdAsync_Found()
        {
            const int id = 5;
            var user = new User { Id = id };
            await _context.AddAsync(user);
            await _context.SaveChangesAsync();

            var res = await _repo.GetAsync(id);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Id, Is.EqualTo(id));
        }



        [Test]
        public async Task GetByIdAsync_NotThere_Null()
        {
            const int id = 5;
            var res = await _repo.GetAsync(id);
            Assert.That(res, Is.Null);
        }


        [Test]
        public async Task GetByEmail_Found()
        {
            const string email = "myemial@explam.co";
            var user = new User { Email = email };
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
            var user = new User()
            {
                Id = id,
                Email = "my@ema.com",
            };

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
            var user = new User() { Id = id, Email = "my@ema.com", };
            var newUser = new User() { Id = id, Email = "myNew@ema.com", };
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
            var newUser = new User() { Id = id, Email = "myNew@ema.com", };
            Assert.ThrowsAsync<UserNotFoundException>(async () =>
                    await _repo.UpdateUserCredentialsAsync(newUser));
        }

        [TestCase(new int[] { })]
        [TestCase(new[] { 1, 4, 32 })]
        public async Task GetUniversitiesForUserAsync_Success(int[] uniIds)
        {
            const int id = 5;
            var unis = uniIds.Select(id => new University { Id = id }).ToList();
            var user = new User
            {
                Id = id,
                Universities = unis
            };
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
            var unis = ids.Select(id => new University { Id = id }).ToList();
            var faculties = ids.Select(id => new Faculty { Name = id.ToString() }).ToList();
            User user = new()
            {
                Id = id,
                Faculties = faculties,
                Universities = unis
            };
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
