using FluentValidation;
using GestoreDeFrotas.Models;
using System;

namespace GestoreDeFrotas.Validators
{
    public class RegistoManutencaoValidator : AbstractValidator<RegistoManutencao>
    {
        public RegistoManutencaoValidator()
        {
            RuleFor(m => m.VeiculoId)
                .GreaterThan(0)
                .WithMessage("O ID do veículo é obrigatório.");

            RuleFor(m => m.Data)
                .LessThanOrEqualTo(DateTime.Now)
                .WithMessage("A data da manutenção não pode ser futura.");

            RuleFor(m => m.Descricao)
                .NotEmpty()
                .WithMessage("A descrição é obrigatória.")
                .MaximumLength(250)
                .WithMessage("A descrição não pode exceder 250 caracteres.");

            RuleFor(m => m.Custo)
                .GreaterThan(0)
                .WithMessage("O custo deve ser superior a zero.");

            RuleFor(m => m.RealizadoPor)
                .NotEmpty()
                .WithMessage("O campo 'Realizado por' é obrigatório.")
                .MaximumLength(100)
                .WithMessage("O campo 'Realizado por' não pode exceder 100 caracteres.");

            RuleFor(m => m.Km)
                .GreaterThanOrEqualTo(0)
                .WithMessage("O KM deve ser igual ou superior a zero.");
        }
    }
}
