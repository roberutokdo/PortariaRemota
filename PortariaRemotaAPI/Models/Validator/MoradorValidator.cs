using FluentValidation;
using System;

namespace PortariaRemotaAPI.Models.Validator
{
    public class MoradorValidator : AbstractValidator<Morador>
    {
        public MoradorValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("Nome do Morador obrigatório.");

            RuleFor(x => x.EMail)
                .NotEmpty().WithMessage("Email do Morador é obrigatório.")
                .EmailAddress().WithMessage("Use um Email válido para o Morador.");

            RuleFor(x => x.Telefone)
                .NotEmpty().WithMessage("Telefone do Morador é obrigatório.");

            RuleFor(x => x.DataNascimento)
                .NotNull().WithMessage("Data de Nascimento do Morador é obrigatório.")
                .Must(ValidarDataNascimento).WithMessage($"Data de Nascimento não pode ser maior que a data atual: {DateTime.Now.Date}");

            RuleFor(x => x.Apartamento)
                .NotNull().WithMessage("É obrigatório o vínculo de um Apartamento ao Morador.");
        }

        private bool ValidarDataNascimento(DateTime dataNascimento)
        {
            return dataNascimento < DateTime.Now;
        }
    }
}
