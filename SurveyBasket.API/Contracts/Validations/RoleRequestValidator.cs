namespace SurveyBasket.API.Contracts.Validations;

public class RoleRequestValidator : AbstractValidator<RoleRequest>
{
    public RoleRequestValidator()
    {
        RuleFor(n => n.Name)
            .NotEmpty();

        RuleFor(p => p.Permissions)
            .NotEmpty()
            .NotNull();

        RuleFor(p => p.Permissions)
            .Must(x => x.Distinct().Count() == x.Count)
            .WithMessage("you cannot add dublicated permissions or the same role")
            .When(x => x.Permissions != null);
    }
}
