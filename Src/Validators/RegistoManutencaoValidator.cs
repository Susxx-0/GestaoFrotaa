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
                .GreaterThan(0).WithMessage("O ID do veículo deve ser um identificador válido.");

            RuleFor(m => m.Data)
                .LessThanOrEqualTo(DateTime.Now).WithMessage("A data da manutenção não pode ser no futuro.");

            RuleFor(m => m.Descricao)
                .NotEmpty().WithMessage("A descrição da manutenção é obrigatória.")
                .MaximumLength(250).WithMessage("A descrição não pode levar mais de 250 caracteres.");

            RuleFor(m => m.Custo)
                .GreaterThan(0).WithMessage("O custo da manutenção deve ser maior do que zero.");

            RuleFor(m => m.RealizadoPor)
                .NotEmpty().WithMessage("É necessário indicar quem realizou a manutenção.")
                .MaximumLength(100).WithMessage("O nome da entidade/técnico não pode ter mais de 100 caracteres.");

            RuleFor(m => m.Km)
                .GreaterThanOrEqualTo(0).WithMessage("Os quilómetros da manutenção não podem ser negativos.");
        }
    }
}