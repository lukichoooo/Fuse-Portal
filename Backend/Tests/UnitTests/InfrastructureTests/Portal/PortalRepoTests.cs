using AutoFixture;
using Core.Entities;
using Core.Entities.Portal;
using Core.Exceptions;
using Core.Interfaces.Portal;
using Infrastructure.Contexts;
using Infrastructure.Repos;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureTests.Portal
{
    [TestFixture]
    public class PortalRepoTests
    {
        private IPortalRepo _sut;
        private MyContext _context;
        private static readonly Fixture _fix = new();

        [OneTimeSetUp]
        public void BeforeAll()
        {
            _fix.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [SetUp]
        public void BeforeEach()
        {
            var options = new DbContextOptionsBuilder<MyContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new MyContext(options);
            _sut = new PortalRepo(_context);
        }

        [TearDown]
        public async Task AfterAllAsync()
        {
            await _context.DisposeAsync();
        }

        [Test]
        public async Task AddSubjectForUserAsync_Success()
        {
            var subject = _fix.Create<Subject>();
            subject.User = null;
            var user = _fix.Create<User>();
            user.Subjects = [];
            subject.UserId = user.Id;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var returnValue = await _sut.AddSubjectForUser(subject);
            var res = await _context.Subjects
                .FirstOrDefaultAsync(s => s.UserId == user.Id);

            Assert.That(res, Is.Not.Null);
            Assert.That(res, Is.EqualTo(returnValue));
        }



        [Test]
        public async Task GetSubjectsPageForUser_Success()
        {
            var subjects = _fix.CreateMany<Subject>().ToList();
            var user = _fix.Create<User>();
            user.Subjects = [];
            foreach (var s in subjects)
            {
                s.User = null;
                s.UserId = user.Id;
            }

            await _context.Users.AddAsync(user);
            await _context.Subjects.AddRangeAsync(subjects);
            await _context.SaveChangesAsync();

            var res = await _sut.GetSubjectsPageForUserAsync(user.Id, null, 111);

            Assert.That(res, Is.Not.Null);
            Assert.That(
                    res.OrderBy(s => s.Name),
                    Is.EqualTo(subjects.OrderBy(s => s.Name))
                    );
        }



        [Test]
        public async Task GetSubjectsFullPageForUserAsync_Success()
        {
            var subject = _fix.Create<Subject>();
            var user = _fix.Create<User>();
            user.Subjects = [];

            subject.UserId = user.Id;
            subject.User = null;
            subject.Tests = _fix.CreateMany<Test>().ToList();
            subject.Schedules = _fix.CreateMany<Schedule>().ToList();
            subject.Lecturers = _fix.CreateMany<Lecturer>().ToList();


            await _context.Users.AddAsync(user);
            await _context.Subjects.AddAsync(subject);
            await _context.SaveChangesAsync();

            var res = await _sut.GetFullSubjectById(subject.Id, user.Id);

            Assert.That(res, Is.Not.Null);
            Assert.That(res.Schedules, Is.Not.Empty);
            Assert.That(res.Tests, Is.Not.Empty);
            Assert.That(res.Lecturers, Is.Not.Empty);
        }


        [Test]
        public async Task RemoveSubjectForUserAsync_Success()
        {
            var subject = _fix.Create<Subject>();
            await _context.Subjects.AddAsync(subject);
            await _context.SaveChangesAsync();

            var returnValue = await _sut.RemoveSubjectById(subject.Id, subject.UserId);
            var res = await _context.Subjects.FindAsync(subject.Id);

            Assert.That(res, Is.Null);
            Assert.That(returnValue, Is.EqualTo(subject));
        }



        [Test]
        public async Task RemoveSubjectForUserAsync_NotFound_Throws()
        {
            var subjectId = _fix.Create<int>();
            Assert.ThrowsAsync<SubjectNotFoundException>(async () =>
                    await _sut.RemoveSubjectById(subjectId, 1));
        }


        [Test]
        public async Task RemoveSubjectForUserAsync_NotAllowed_Throws()
        {
            const int realUserId = 5, fakeUserId = 10;
            var subject = _fix.Create<Subject>();
            subject.UserId = realUserId;

            await _context.Subjects.AddAsync(subject);
            await _context.SaveChangesAsync();

            Assert.ThrowsAsync<SubjectNotFoundException>(async () =>
                    await _sut.RemoveSubjectById(subject.Id, fakeUserId));
        }

    }
}
