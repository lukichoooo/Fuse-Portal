using Core.Dtos;
using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;
using Infrastructure.Contexts;
using Infrastructure.Repos;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureTests
{
    [TestFixture]
    public class UniversityRepoTests
    {
        private IUniversityRepo _repo;
        private MyContext _context;

        [OneTimeSetUp]
        public void BeforeAll()
        {
            var options = new DbContextOptionsBuilder<MyContext>()
                .UseInMemoryDatabase("MyInMem")
                .Options;
            _context = new MyContext(options);
            _repo = new UniversityRepo(_context);
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

            _context.Universities.RemoveRange(_context.Universities);
            _context.Users.RemoveRange(_context.Users);

            await _context.SaveChangesAsync();
        }

        [TestCase(new[] { 1, 3, 5, 6 })]
        [TestCase(new int[] { })]
        public async Task GetAllAsync_Success(int[] ids)
        {
            const int pageSize = 16;
            var universities = ids.Select(id => new University { Id = id });
            await _context.Universities.AddRangeAsync(universities);
            await _context.SaveChangesAsync();

            var res = await _repo.GetPageAsync(-1, pageSize);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Select(u => u.Id).Order(),
                    Is.EquivalentTo(universities.Select(u => u.Id).Order()));
        }


        [TestCase(5)]
        [TestCase(20)]
        public async Task GetAllAsync_Paged_Success(int lastId)
        {
            const int pageSize = 16;
            var unis = Enumerable.Range(1, 25)
                    .Reverse()
                    .Select(id =>
                    new University { Id = id }).ToList();
            await _context.Universities.AddRangeAsync(unis);
            await _context.SaveChangesAsync();

            var res = await _repo.GetPageAsync(lastId, pageSize);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Min(u => u.Id), Is.EqualTo(lastId + 1));
        }


        [Test]
        public async Task GetByIdAsync_Success()
        {
            const int id = 5;
            var uni = new University { Id = 5 };
            await _context.Universities.AddAsync(uni);
            await _context.SaveChangesAsync();

            var res = await _repo.GetByIdAsync(id);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Id, Is.EqualTo(id));
        }


        [Test]
        public async Task GetByIdAsync_NotFound_Null()
        {
            const int id = 5;

            var res = await _repo.GetByIdAsync(id);

            Assert.That(res, Is.Null);
        }


        [Test]
        public async Task CreateAsync_Success()
        {
            const int id = 5;
            var uni = new University { Id = 5 };

            var rv = await _repo.CreateAsync(uni);
            var res = await _context.Universities.FirstOrDefaultAsync(u =>
                    u.Id == id);

            Assert.Multiple(() =>
            {
                Assert.That(res, Is.Not.Null);
                Assert.That(rv, Is.Not.Null);
            });
            Assert.That(rv, Is.EqualTo(res));
        }


        [Test]
        public async Task UpdateAsync_Success()
        {
            const int id = 5;
            var uni = new University { Id = id, Name = "OldName" };
            var newUni = new University { Id = id, Name = "NewName" };
            await _context.Universities.AddAsync(uni);
            await _context.SaveChangesAsync();

            var rv = await _repo.UpdateAsync(newUni);
            var res = await _context.Universities.FirstOrDefaultAsync(u =>
                    u.Id == id);

            Assert.Multiple(() =>
            {
                Assert.That(res, Is.Not.Null);
                Assert.That(rv, Is.Not.Null);
            });
            Assert.Multiple(() =>
            {
                Assert.That(rv, Is.EqualTo(res));
                Assert.That(res.Name, Is.EqualTo(newUni.Name));
            });
        }


        [Test]
        public async Task UpdateAsync_NotFound_Throws()
        {
            const int id = 5;
            var newUni = new University { Id = id, Name = "NewName" };

            Assert.ThrowsAsync<UniversityNotFoundException>(async () =>
                    await _repo.UpdateAsync(newUni));
        }


        [Test]
        public async Task DeleteByIdAsyc_Success()
        {
            const int id = 5;
            var uni = new University { Id = id, Name = "Namos" };
            await _context.Universities.AddAsync(uni);
            await _context.SaveChangesAsync();

            var rv = await _repo.DeleteByIdAsync(id);
            var res = await _context.Universities.FirstOrDefaultAsync(u =>
                    u.Id == id);

            Assert.Multiple(() =>
            {
                Assert.That(res, Is.Null);
                Assert.That(rv, Is.Not.Null);
            });
            Assert.That(rv, Is.EqualTo(uni));
        }


        [Test]
        public async Task DeleteByIdAsync_NotFound_Throws()
        {
            const int id = 5;
            Assert.ThrowsAsync<UniversityNotFoundException>(async () =>
                    await _repo.DeleteByIdAsync(id));
        }


        [TestCase("luka")]
        [TestCase("user")]
        [TestCase("test")]
        [TestCase("")]
        public async Task PageByNameAsync(string name)
        {
            var unis = new List<University>()
            {
                new (){ Name = "luka" },
                new (){ Name = "user" },
                new (){ Name = "test" },
                new (){ Name = "lukaaa" },
                new (){ Name = "iluka" },
                new (){ Name = "ilika" },
                new (){ Name = "LukA" },
            };
            await _context.Universities.AddRangeAsync(unis);
            await _context.SaveChangesAsync();

            var expected = await _context.Universities.Where(s =>
                    s.Name.Contains(name, StringComparison.CurrentCultureIgnoreCase))
                    .ToArrayAsync();
            var res = await _repo.PageByNameAsync(name);

            Assert.That(res, Is.Not.Null);
            Assert.That(res, Is.EquivalentTo(expected));
        }



        [Test]
        public async Task PageByName_Paged_Success()
        {
            const int pageSize = 16;
            var unis = Enumerable.Range(1, 25)
                .Reverse()
                .Select(id => new University { Id = id, Name = "tsU" })
                .ToList();
            await _context.Universities.AddRangeAsync(unis);
            await _context.SaveChangesAsync();

            var res = await _repo.PageByNameAsync(
                    name: "TSU",
                    pageSize: pageSize);

            Assert.That(res, Is.Not.Null);
            Assert.That(res,
                    Is.EquivalentTo(unis.OrderBy(u => u.Id)
                        .Take(pageSize)));
        }



        [TestCase(new[] { 1, 2, 5 })]
        [TestCase(new int[] { })]
        public async Task GetUsersByUniversityIdAsync_Success(int[] ids)
        {
            var users = ids.Select(id => new User { Id = id }).ToList();
            const int id = 1;
            var uni = new University()
            {
                Id = id,
                Name = "uniName",
                Users = users
            };
            await _context.Universities.AddAsync(uni);
            await _context.SaveChangesAsync();

            var res = await _repo.GetUsersPageAsync(id, -1, 16);

            Assert.That(res, Is.Not.Null);
            Assert.That(res, Is.EquivalentTo(users));
        }


        [Test]
        public async Task GetUsersByUniversityIdAsync_Success()
        {
            const int id = 1;
            Assert.ThrowsAsync<UniversityNotFoundException>(async () =>
                    await _repo.GetUsersPageAsync(id, -1, 16));
        }
    }
}
