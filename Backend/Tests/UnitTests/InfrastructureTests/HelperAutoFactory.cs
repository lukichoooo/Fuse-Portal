using AutoFixture;
using Core.Dtos;
using Core.Entities;
using Core.Entities.Portal;

namespace UnitTests
{
    public static class HelperAutoFactory
    {
        private const int DefaultRepeatCount = 3;

        public static Fixture Fix { get; }
            = new() { RepeatCount = DefaultRepeatCount };

        static HelperAutoFactory()
        {
            Fix.Behaviors.Add(new OmitOnRecursionBehavior());
        }


        public static User CreateUser(int? id = null)
            => Fix.Build<User>()
                .With(u => u.Id, id)
                .Create();

        public static University CreateUniversity(int? id = null)
            => Fix.Build<University>()
                .With(uni => uni.Id, id ?? Fix.Create<int>())
                .Create();


        public static List<University> CreateUniversityList(int repeatCount)
        {
            var fixture = new Fixture { RepeatCount = repeatCount };
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            return fixture.Build<University>()
                .CreateMany()
                .ToList();
        }
        public static List<Course> CreateCourseList(int repeatCount)
        {
            var fixture = new Fixture { RepeatCount = repeatCount };
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            return Fix.Build<Course>()
                .CreateMany()
                .ToList();
        }

        public static UserPrivateDto CreateUserPrivateDto(
                int? id = null,
                        string? name = null,
                        string? email = null,
                        string? password = null
                    )
            => Fix.Build<UserPrivateDto>()
            .With(u => u.Id, id)
            .With(u => u.Name, name)
            .With(u => u.Email, email)
            .With(u => u.Password, password)
            .Create();
    }
}
