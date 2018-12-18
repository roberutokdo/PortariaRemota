using FluentValidation;
namespace PortariaRemotaAPI.Models.Validator
{
    public class ApartamentoValidator : AbstractValidator<Apartamento>
    {
        public ApartamentoValidator()
        {
            RuleFor(x => x.Bloco)
                .NotEmpty().WithMessage("Bloco do Apartamento é obrigatório.");

            RuleFor(x => x.Numero)
                .GreaterThan(0).WithMessage("O Número do Apartamento deve ser maior que 0 (zero).");
        }
    }
}
