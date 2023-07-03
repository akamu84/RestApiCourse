using FluentValidation;
using Movies.Application.Models;

namespace Movies.Application.Validators;

public class GetAllMoviesOptionsValidator : AbstractValidator<GetAllMoviesOptions>
{
    private static readonly string[] AcceptableSortFields = { "title", "yearofrelease" };
    public GetAllMoviesOptionsValidator()
    {
        RuleFor(x => x.YearOfRelease).LessThanOrEqualTo(DateTime.UtcNow.Year);
        
        RuleFor(x => x.SortField).Must(x => x is null || AcceptableSortFields.Contains(x, StringComparer.OrdinalIgnoreCase))
            .WithMessage($"Sort field must be one of the following: {string.Join(", ", AcceptableSortFields)}");

        RuleFor(x => x.PageSize).InclusiveBetween(1, 25).WithMessage("You can only request between 1 and 25 movies at a time");
    }
}