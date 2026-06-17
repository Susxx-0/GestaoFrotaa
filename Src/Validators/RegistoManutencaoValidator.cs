using FluentValidation;
using GestoreDeFrotas.Models;
using System;

namespace GestoreDeFrotas.Validators
{
    public class RegistoManutencaoValidator : AbstractValidator<RegistoManutencao>
    {
        public RegistoManutencaoValidator()
        {
            RuleFor(m => m.VeiculoId).GreaterThan(0);
            RuleFor(m => m.Data).LessThanOrEqualTo(DateTime.Now);
            RuleFor(m => m.Descricao).NotEmpty().MaximumLength(250);
            RuleFor(m => m.Custo).GreaterThan(0);
            RuleFor(m => m.RealizadoPor).NotEmpty().MaximumLength(100);
            RuleFor(m => m.Km).GreaterThanOrEqualTo(0);
        }
    }
}