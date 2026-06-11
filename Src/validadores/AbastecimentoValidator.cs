using FluentValidation;
using GestoreDeFrotas.Models;
using System;

namespace GestoreDeFrotas.Validators
{
    public class AbastecimentoValidator : AbstractValidator<Abastecimento>
    {
        // CORREGIDO: Constructor limpio, sin errores de sintaxis
        public AbastecimentoValidator()
        {
            RuleFor(a => a.VeiculoId)
                .GreaterThan(0).WithMessage("Deve selecionar um veículo válido.");

          
            RuleFor(a => a.Litros)
                .GreaterThan(0).WithMessage("A quantidade de litros deve ser maior que zero.");

            RuleFor(a => a.PrecoPorLitro)
                .GreaterThan(0).WithMessage("O preço por litro deve ser maior que zero.");

            RuleFor(a => a.KmAtual)
                .GreaterThan(0).WithMessage("Os quilómetros atuais devem ser maiores que zero.");

            RuleFor(a => a.Combustivel)
                .NotEmpty().WithMessage("O tipo de combustível é obrigatório.");
            RuleFor(a => a.Posto)
                .NotEmpty().WithMessage("O posto de combustível é obrigatório.");

          
            RuleFor(a => a.Data)
                .LessThanOrEqualTo(DateTime.Now).WithMessage("A data do abastecimento não pode ser no futuro.");
        }
    }
}