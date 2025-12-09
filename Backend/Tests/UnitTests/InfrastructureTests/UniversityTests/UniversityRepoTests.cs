using AutoFixture;
using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;
using Infrastructure.Contexts;
using Infrastructure.Repos;
using Microsoft.EntityFrameworkCore;
using UnitTests;

namespace InfrastructureTests
{
    [TestFixture]
    public class UniversityRepoTests
    {
        private IUniversityRepo _repo;
        private MyContext _context;

        [SetUp]
        public void BeforeEach()
        {
            var options = new DbContextOptionsBuilder<MyContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new MyContext(options);
            _repo = new UniversityRepo(_context);
        }

        [TearDown]
        public async Task AfterEach()
        {
            await _context.DisposeAsync();
        }

        [TestCase(0)]
        [TestCase(3)]
        public async Task GetAllAsync_Success(int repeatCount)
        {
            const int pageSize = 16;
            var fixture = new Fixture() { RepeatCount = repeatCount };
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var unis = fixture.Build<University>()
                .With(uni => uni.UserUniversities, [])
                .CreateMany()
                .ToList();
            await _context.Universities.AddRangeAsync(unis);
            await _context.SaveChangesAsync();

            var res = await _repo.GetPageAsync(int.MinValue, pageSize);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Select(u => u.Id).Order(),
                    Is.EquivalentTo(unis.Select(u => u.Id).Order()));
        }


        [TestCase(5)]
        [TestCase(20)]
        public async Task GetAllAsync_Paged_Success(int lastId)
        {
            const int pageSize = 16;
            List<University> unis = Enumerable.Range(1, 25)
                    .Reverse()
                    .Select(id => HelperAutoFactory.CreateUniversity(id)
                    ).ToList();

            await _context.Universities.AddRangeAsync(unis);
            await _context.SaveChangesAsync();

            var res = await _repo.GetPageAsync(lastId, pageSize);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Min(u => u.Id), Is.EqualTo(lastId + 1));
        }



        [TestCase(16)]
        [TestCase(8)]
        public async Task GetPageAsync_Paged_PagingTest(int pageSize)
        {
            const int n = 33;
            int? lastId = null;
            var unis = Enumerable.Range(1, n)
                    .Reverse()
                    .Select(id => HelperAutoFactory.CreateUniversity(id))
                    .ToList();
            await _context.AddRangeAsync(unis);
            await _context.SaveChangesAsync();

            HashSet<int> seenId = [];
            for (int i = 0; i < n; i += pageSize)
            {
                var res = await _repo.GetPageAsync(lastId, pageSize);
                Assert.That(res, Is.Not.Null);
                foreach (var u in res)
                {
                    Assert.That(seenId.Contains(u.Id), Is.EqualTo(false));
                    seenId.Add(u.Id);
                }
                lastId = res.Last().Id;
            }
            Assert.That(seenId, Has.Count.EqualTo(n));
        }


        [Test]
        public async Task GetByIdAsync_Success()
        {
            const int id = 5;
            var uni = HelperAutoFactory.CreateUniversity(id);
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
            var uni = HelperAutoFactory.CreateUniversity(id);

            var rv = await _repo.CreateAsync(uni);
            var res = await _context.Universities
                .FirstOrDefaultAsync(u => u.Id == id);

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
            var uni = HelperAutoFactory.CreateUniversity(5);
            uni.Name = "oldUni";
            var newUni = HelperAutoFactory.CreateUniversity(5);
            newUni.Name = "newUni";
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
            var newUni = HelperAutoFactory.CreateUniversity(id);

            Assert.ThrowsAsync<UniversityNotFoundException>(async () =>
                    await _repo.UpdateAsync(newUni));
        }


        [Test]
        public async Task DeleteByIdAsyc_Success()
        {
            const int id = 5;
            var uni = HelperAutoFactory.CreateUniversity(id);
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
        public async Task GetPageByNameAsync_SearchName_Success(string name)
        {
            const int n = 33;
            int? lastId = null;
            const int pageSize = 10;
            var fixture = new Fixture();
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());


            var unis = Enumerable.Range(0, 33)
                .ToList()
                .ConvertAll(_ =>
                    {
                        var uni = fixture.Create<University>();
                        uni.Name = name;
                        return uni;
                    });


            await _context.Universities.AddRangeAsync(unis);
            await _context.SaveChangesAsync();

            HashSet<int> seenId = [];
            for (int i = 0; i < n; i += pageSize)
            {
                var res = await _repo.GetPageByNameAsync(name, lastId, pageSize);
                Assert.That(res, Is.Not.Null);
                foreach (var u in res)
                {
                    Assert.That(seenId.Contains(u.Id), Is.EqualTo(false));
                    seenId.Add(u.Id);
                }
                lastId = res.Last().Id;
            }
            Assert.That(seenId, Has.Count.EqualTo(n));
        }


        [TestCase(0)]
        [TestCase(3)]
        public async Task GetPageByName_Paged_Success(int repeatCount)
        {
            const int lastId = int.MinValue, pageSize = 16;
            const string name = "";
            var fixture = new Fixture() { RepeatCount = repeatCount };
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var unis = fixture.CreateMany<University>().ToList();
            await _context.Universities.AddRangeAsync(unis);
            await _context.SaveChangesAsync();

            var res = await _repo.GetPageByNameAsync(name, lastId, pageSize);

            Assert.That(res, Is.Not.Null);
            Assert.That(res
                    .ConvertAll(uni => uni.Name)
                    .Order(),
                    Is.EquivalentTo(unis
                        .ConvertAll(uni => uni.Name)
                        .Order()
                        .Take(pageSize)));
        }



        [TestCase("luka")]
        [TestCase("user")]
        [TestCase("LUKAA")]
        [TestCase("")]
        public async Task GetPageByNameAsync_SearchName_PagingTest(string name)
        {
            const int lastId = int.MinValue, pageSize = 16;
            var fixture = new Fixture() { RepeatCount = 3 };
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var unis = fixture.Build<University>()
                .With(uni => uni.Name, "LuKA")
                .CreateMany()
                .ToList();

            await _context.Universities.AddRangeAsync(unis);
            await _context.SaveChangesAsync();

            var expected = await _context.Universities
                .Where(u => u.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();

            var res = await _repo.GetPageByNameAsync(name, lastId, pageSize);

            Assert.That(res, Is.Not.Null);
            Assert.That(res
                    .ConvertAll(uni => uni.Name)
                    .Order(),
                    Is.EquivalentTo(expected
                        .ConvertAll(uni => uni.Name)
                        .Order()));
        }

        [Test]
        public async Task GetByNameAsync_Success()
        {
            var fixture = new Fixture();
            var name = fixture.Create<string>();
            var uni = HelperAutoFactory.CreateUniversity();
            uni.Name = name;

            await _context.AddAsync(uni);
            await _context.SaveChangesAsync();

            var res = await _repo.GetByNameAsync(name);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Name, Is.EqualTo(uni.Name));
        }


        [Test]
        public async Task GetByNameAsync_NotThere_ReturnNull()
        {
            var fixture = new Fixture();
            var name = fixture.Create<string>();
            var res = await _repo.GetByNameAsync(name);

            Assert.That(res, Is.Null);
        }
    }
}
