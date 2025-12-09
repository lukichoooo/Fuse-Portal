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

            var returnValue = await _sut.AddSubjectForUserAsync(subject);
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

        [TestCase(0)]
        [TestCase(22)]
        [TestCase(16)]
        public async Task GetSubjectsPageForUser_PagingTest(int n)
        {
            const int pageSize = 16;
            int? lastId = null;
            const int userId = 10;


            var fixture = new Fixture();
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var subjects = Enumerable.Range(0, n)
                .ToList()
                .ConvertAll(_ =>
                {
                    var chat = fixture.Create<Subject>();
                    chat.UserId = userId;
                    chat.User = null;
                    return chat;
                });

            await _context.Subjects.AddRangeAsync(subjects);
            await _context.SaveChangesAsync();

            HashSet<int> seenId = [];
            for (int i = 0; i < n; i += pageSize)
            {
                var res = await _sut.GetSubjectsPageForUserAsync(
                    userId, lastId, pageSize);
                Assert.That(res, Is.Not.Null);
                foreach (var chat in res)
                {
                    Assert.That(seenId.Contains(chat.Id), Is.EqualTo(false));
                    seenId.Add(chat.Id);
                }
                if (res.Count > 0)
                    lastId = res[^1].Id;
            }
            Assert.That(seenId, Has.Count.EqualTo(n));
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

            var res = await _sut.GetFullSubjectByIdAsync(subject.Id, user.Id);

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

            await _context.Subjects.AddAsync(subject);
            await _context.SaveChangesAsync();
            subject.UserId = realUserId;
            await _context.SaveChangesAsync();

            Assert.ThrowsAsync<SubjectNotFoundException>(async () =>
                    await _sut.RemoveSubjectById(subject.Id, fakeUserId));
        }


        [Test]
        public async Task AddScheduleForSubjectAsync_Success()
        {
            var user = _fix.Create<User>();
            var subject = _fix.Create<Subject>();

            await _context.Users.AddAsync(user);
            await _context.Subjects.AddAsync(subject);
            await _context.SaveChangesAsync();
            subject.User = null;
            user.Subjects = [];
            subject.UserId = user.Id;
            await _context.SaveChangesAsync();

            var schedule = _fix.Create<Schedule>();
            schedule.Subject = null;
            schedule.SubjectId = subject.Id;

            var rv = await _sut.AddScheduleForSubjectAsync(schedule, user.Id);
            var res = await _context.Schedules
                .FirstOrDefaultAsync(s => s.Id == schedule.Id
                        && s.SubjectId == subject.Id);

            Assert.That(rv, Is.Not.Null);
            Assert.That(res, Is.Not.Null);
            Assert.That(res, Is.EqualTo(schedule));
            Assert.That(rv, Is.EqualTo(schedule));
        }

        [Test]
        public async Task AddScheduleForSubjectAsync_WrongUser_Throws()
        {
            const int realUserId = 5, fakeUserId = 10;
            var user = _fix.Create<User>();
            var subject = _fix.Create<Subject>();

            await _context.Users.AddAsync(user);
            await _context.Subjects.AddAsync(subject);
            subject.UserId = user.Id = realUserId;
            await _context.SaveChangesAsync();

            var schedule = _fix.Create<Schedule>();
            schedule.Subject = null;
            schedule.SubjectId = subject.Id;

            Assert.ThrowsAsync<SubjectNotFoundException>(async () =>
                    await _sut.AddScheduleForSubjectAsync(schedule, fakeUserId));
        }


        [Test]
        public async Task RemoveScheduleByIdAsync_Success()
        {
            var user = _fix.Create<User>();
            var subject = _fix.Create<Subject>();

            await _context.Users.AddAsync(user);
            await _context.Subjects.AddAsync(subject);
            await _context.SaveChangesAsync();
            subject.UserId = user.Id;

            var schedule = _fix.Create<Schedule>();
            schedule.Subject = null;
            schedule.SubjectId = subject.Id;
            await _context.AddAsync(schedule);
            await _context.SaveChangesAsync();

            var rv = await _sut.RemoveScheduleByIdAsync(schedule.Id, user.Id);
            var res = await _context.Schedules
                .FirstOrDefaultAsync(s => s.Id == schedule.Id
                        && s.SubjectId == subject.Id);

            Assert.That(rv, Is.Not.Null);
            Assert.That(res, Is.Null);
            Assert.That(rv, Is.EqualTo(schedule));
        }



        [Test]
        public async Task RemoveScheduleByIdAsync_WrongUser_Throws()
        {
            const int realUserId = 5, fakeUserId = 10;
            var user = _fix.Create<User>();
            var subject = _fix.Create<Subject>();

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            subject.Id = user.Id = realUserId;
            var scheduleId = _fix.Create<int>();

            Assert.ThrowsAsync<ScheduleNotFoundException>(async () =>
                    await _sut.RemoveScheduleByIdAsync(scheduleId, fakeUserId));
        }

        [Test]
        public async Task RemoveScheduleByIdAsync_SubjectNotFound_Throws()
        {
            var user = _fix.Create<User>();
            var subject = _fix.Create<Subject>();

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            subject.Id = user.Id;
            var scheduleId = _fix.Create<int>();

            Assert.ThrowsAsync<ScheduleNotFoundException>(async () =>
                    await _sut.RemoveScheduleByIdAsync(scheduleId, user.Id));
        }



        [Test]
        public async Task AddLecturerToSubjectAsync_Success()
        {
            var user = _fix.Create<User>();
            var subject = _fix.Create<Subject>();

            await _context.Users.AddAsync(user);
            await _context.Subjects.AddAsync(subject);
            await _context.SaveChangesAsync();
            subject.UserId = user.Id;
            await _context.SaveChangesAsync();

            var lecturer = _fix.Create<Lecturer>();
            lecturer.SubjectId = subject.Id;
            lecturer.Subject = null;

            var rv = await _sut.AddLecturerToSubjectAsync(lecturer, user.Id);
            var res = await _context.Lecturers
                .FirstOrDefaultAsync(l => l.Id == lecturer.Id
                        && l.SubjectId == subject.Id);

            Assert.That(rv, Is.Not.Null);
            Assert.That(res, Is.Not.Null);
            Assert.That(res, Is.EqualTo(lecturer));
            Assert.That(rv, Is.EqualTo(lecturer));
        }


        [Test]
        public async Task AddLecturerToSubjectAsync_WrongUser_Throws()
        {
            const int realUserId = 5, fakeUserId = 10;
            var user = _fix.Create<User>();
            var subject = _fix.Create<Subject>();

            await _context.Users.AddAsync(user);
            await _context.Subjects.AddAsync(subject);
            subject.UserId = user.Id = realUserId;
            await _context.SaveChangesAsync();

            var lecturer = _fix.Create<Lecturer>();
            lecturer.SubjectId = subject.Id;
            lecturer.Subject = null;

            Assert.ThrowsAsync<SubjectNotFoundException>(async () =>
                     await _sut.AddLecturerToSubjectAsync(lecturer, fakeUserId));
        }


        [Test]
        public async Task RemoveLecturerByIdAsync_Success()
        {
            var user = _fix.Create<User>();
            var subject = _fix.Create<Subject>();

            await _context.Users.AddAsync(user);
            await _context.Subjects.AddAsync(subject);
            await _context.SaveChangesAsync();
            subject.UserId = user.Id;
            await _context.SaveChangesAsync();

            var lecturer = _fix.Create<Lecturer>();
            lecturer.SubjectId = subject.Id;
            lecturer.Subject = null;
            await _context.AddAsync(lecturer);
            await _context.SaveChangesAsync();

            var rv = await _sut.RemoveLecturerByIdAsync(lecturer.Id, user.Id);
            var res = await _context.Lecturers
                .FirstOrDefaultAsync(l => l.Id == lecturer.Id
                        && l.SubjectId == subject.Id);

            Assert.That(rv, Is.Not.Null);
            Assert.That(res, Is.Null);
            Assert.That(rv, Is.EqualTo(lecturer));
        }


        [Test]
        public async Task RemoveLecturerByIdAsync_SubjectNotFound_Throws()
        {
            var user = _fix.Create<User>();
            var subject = _fix.Create<Subject>();

            await _context.Users.AddAsync(user);
            subject.Id = user.Id;
            await _context.SaveChangesAsync();
            var lecturerId = _fix.Create<int>();

            Assert.ThrowsAsync<LecturerNotFoundException>(async () =>
                    await _sut.RemoveLecturerByIdAsync(lecturerId, user.Id));
        }


        [Test]
        public async Task AddTestToSubjectAsync_Success()
        {
            var user = _fix.Create<User>();
            var subject = _fix.Create<Subject>();

            await _context.Users.AddAsync(user);
            await _context.Subjects.AddAsync(subject);
            await _context.SaveChangesAsync();
            subject.UserId = user.Id;
            await _context.SaveChangesAsync();

            var test = _fix.Create<Test>();
            test.SubjectId = subject.Id;
            test.Subject = null;

            var rv = await _sut.AddTestForSubjectAsync(test, user.Id);
            var res = await _context.Tests
                .FirstOrDefaultAsync(t => t.Id == test.Id
                        && t.SubjectId == subject.Id);

            Assert.That(rv, Is.Not.Null);
            Assert.That(res, Is.Not.Null);
            Assert.That(res, Is.EqualTo(test));
            Assert.That(rv, Is.EqualTo(test));
        }


        [Test]
        public async Task AddTestToSubjectAsync_SubjectNotFound_Throws()
        {
            const int realUserId = 5, fakeUserId = 10;
            var user = _fix.Create<User>();
            var subject = _fix.Create<Subject>();

            await _context.Users.AddAsync(user);
            await _context.Subjects.AddAsync(subject);
            await _context.SaveChangesAsync();
            subject.UserId = user.Id = realUserId;

            var test = _fix.Create<Test>();
            test.SubjectId = subject.Id;
            test.Subject = null;

            Assert.ThrowsAsync<SubjectNotFoundException>(async () =>
                    await _sut.AddTestForSubjectAsync(test, fakeUserId));
        }


        [Test]
        public async Task RemoveTestByIdAsync_Success()
        {
            var user = _fix.Create<User>();
            var subject = _fix.Create<Subject>();

            await _context.Users.AddAsync(user);
            await _context.Subjects.AddAsync(subject);
            await _context.SaveChangesAsync();
            subject.UserId = user.Id;

            var test = _fix.Create<Test>();
            test.SubjectId = subject.Id;
            test.Subject = null;
            await _context.AddAsync(test);
            await _context.SaveChangesAsync();

            var rv = await _sut.RemoveTestByIdAsync(test.Id, user.Id);
            var res = await _context.Tests
                .FirstOrDefaultAsync(t => t.Id == test.Id
                        && t.SubjectId == subject.Id);

            Assert.That(rv, Is.Not.Null); Assert.That(res, Is.Null);
            Assert.That(rv, Is.EqualTo(test));
        }


        [Test]
        public async Task RemoveTestByIdAsync_WrongUser_Throws()
        {
            const int realUserId = 5, fakeUserId = 10;
            var user = _fix.Create<User>();
            var subject = _fix.Create<Subject>();
            subject.UserId = user.Id = realUserId;

            await _context.Users.AddAsync(user);
            await _context.Subjects.AddAsync(subject);
            await _context.SaveChangesAsync();

            var test = _fix.Create<Test>();
            test.SubjectId = subject.Id;
            test.Subject = null;
            await _context.AddAsync(test);
            await _context.SaveChangesAsync();

            Assert.ThrowsAsync<TestNotFoundException>(async () =>
                     await _sut.RemoveTestByIdAsync(test.Id, fakeUserId));
        }

        [Test]
        public async Task RemoveTestByIdAsync_NotFound_Throws()
        {
            var userId = _fix.Create<int>();
            var testId = _fix.Create<int>();

            Assert.ThrowsAsync<TestNotFoundException>(async () =>
                    await _sut.RemoveTestByIdAsync(testId, userId));
        }


        [Test]
        public async Task GetFullTestByIdAsync_Success()
        {
            var user = _fix.Create<User>();
            var subject = _fix.Create<Subject>();

            await _context.Users.AddAsync(user);
            await _context.Subjects.AddAsync(subject);
            await _context.SaveChangesAsync();
            subject.UserId = user.Id;

            var test = _fix.Create<Test>();
            test.SubjectId = subject.Id;
            test.Subject = null;
            await _context.AddAsync(test);
            await _context.SaveChangesAsync();

            var res = await _sut.GetFullTestByIdAsync(test.Id, user.Id);

            Assert.That(res, Is.Not.Null);
            Assert.That(res, Is.EqualTo(test));
        }


        [Test]
        public async Task GetFullTestByIdAsync_WrongUser_Throws()
        {
            const int realUserId = 5, fakeUserId = 10;
            var user = _fix.Create<User>();
            var subject = _fix.Create<Subject>();
            subject.UserId = user.Id = realUserId;

            await _context.Users.AddAsync(user);
            await _context.Subjects.AddAsync(subject);
            await _context.SaveChangesAsync();

            var test = _fix.Create<Test>();
            test.SubjectId = subject.Id;
            test.Subject = null;
            await _context.AddAsync(test);
            await _context.SaveChangesAsync();

            Assert.ThrowsAsync<TestNotFoundException>(async () =>
                     await _sut.GetFullTestByIdAsync(test.Id, fakeUserId));
        }

        [Test]
        public async Task GetFullTestByIdAsync_NotFound_Throws()
        {
            var userId = _fix.Create<int>();
            var testId = _fix.Create<int>();

            Assert.ThrowsAsync<TestNotFoundException>(async () =>
                    await _sut.GetFullTestByIdAsync(testId, userId));
        }

    }
}
