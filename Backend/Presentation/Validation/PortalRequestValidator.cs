using Core.Dtos;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Presentation.Validation;

public class ScheduleRequestDtoValidator : AbstractValidator<ScheduleRequestDto>
{
    public ScheduleRequestDtoValidator(IOptions<ValidatorSettings> options)
    {
        RuleFor(s => s.Location)
            .Length(0, 80);
        RuleFor(s => s.Metadata)
            .Length(0, 100)
            .Unless(s => s.Metadata is null);
    }
}


public class LecturerRequestDtoValidator : AbstractValidator<LecturerRequestDto>
{
    public LecturerRequestDtoValidator(IOptions<ValidatorSettings> options)
    {
        RuleFor(l => l.Name)
            .Length(1, 30);
    }
}


public class TestRequestDtoValidator : AbstractValidator<TestRequestDto>
{
    public TestRequestDtoValidator(IOptions<ValidatorSettings> options)
    {
        RuleFor(l => l.Name)
            .Length(1, 30);
        RuleFor(l => l.Metadata)
            .Length(0, 100)
            .Unless(l => l.Metadata is null);
    }
}

