using AutoFixture;
using Core.Dtos;
using Core.Entities;
using Core.Enums;

namespace UnitTests
{
    public static class HelperAutoFactory
    {
        private const int DefaultRepeatCount = 3;

        private static readonly Fixture _fix = new() { RepeatCount = DefaultRepeatCount };


        public static User CreateUser(int? id = null)
            => _fix.Build<User>()
                .With(u => u.Universities, [])
                .With(u => u.Courses, [])
                .With(u => u.Address, _fix.Create<Address>())
                .With(u => u.Chats, [])
                .With(u => u.Id, id)
                .Create();

        public static University CreateUniversity(int? id = null)
            => _fix.Build<University>()
                .With(uni => uni.Id, id ?? _fix.Create<int>())
                .With(uni => uni.Users, [])
                .With(uni => uni.Address, _fix.Create<Address>())
                .Create();


        public static List<University> CreateUniversityList(int repeatCount)
        {
            _fix.RepeatCount = repeatCount;
            var res = _fix.Build<University>()
                .With(uni => uni.Users, [])
                .With(uni => uni.Address, _fix.Create<Address>())
                .CreateMany()
                .ToList();
            _fix.RepeatCount = DefaultRepeatCount;
            return res;
        }
        public static List<Course> CreateCourseList(int repeatCount)
        {
            _fix.RepeatCount = repeatCount;
            var res = _fix.Build<Course>()
                .With(uni => uni.Users, [])
                .CreateMany()
                .ToList();
            _fix.RepeatCount = DefaultRepeatCount;
            return res;
        }

        public static UserPrivateDto CreateUserPrivateDto(
                int? id = null,
                        string? name = null,
                        string? email = null,
                        string? password = null,
                        Address? address = null
                    )
            => new()
            {
                Id = id ?? 1,
                Name = name ?? string.Empty,
                Email = email ?? string.Empty,
                Password = password ?? string.Empty,
                Address = address ?? new()
                {
                    City = "NY",
                    CountryA3 = CountryCode.GEO
                }
            };
    }
}
