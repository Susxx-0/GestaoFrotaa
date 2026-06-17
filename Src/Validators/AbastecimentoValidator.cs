using FluentValidation;
using GestoreDeFrotas.Models;
using System;

namespace GestoreDeFrotas.Validators
{
    public class AbastecimentoValidator : AbstractValidator<Abastecimento>
    {
        public AbastecimentoValidator()
        {
            RuleFor(a => a.VeiculoId).GreaterThan(0).WithMessage("Selecione um veículo válido.");
            RuleFor(a => a.Litros).GreaterThan(0).WithMessage("Litros devem ser maiores que zero.");
            RuleFor(a => a.PrecoPorLitro).GreaterThan(0).WithMessage("Preço por litro inválido.");
            RuleFor(a => a.KmAtual).GreaterThan(0).WithMessage("Quilometragem atual inválida.");
            RuleFor(a => a.Combustivel).NotEmpty().WithMessage("Combustível é obrigatório.");
            RuleFor(a => a.Posto).NotEmpty().WithMessage("Posto é obrigatório.");
            RuleFor(a => a.Data).LessThanOrEqualTo(DateTime.Now).WithMessage("Data do abastecimento inválida.");
        }
    }
}