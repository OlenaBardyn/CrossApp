using FluentValidation;

namespace Api.Contracts;

public class SaveBookRequestValidator : AbstractValidator<SaveBookRequest>
{
    public SaveBookRequestValidator()
    {
        RuleFor(x => x.Isbn).NotEmpty().WithMessage("ISBN обов'язковий.");
        RuleFor(x => x.Title).NotEmpty().WithMessage("Назва обов'язкова.").MaximumLength(200);
        RuleFor(x => x.Year).InclusiveBetween(1450, DateTime.Now.Year);
    }
}