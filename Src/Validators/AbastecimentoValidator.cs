using FluentValidation;
using GestoreDeFrotas.Models;

namespace GestoreDeFrotas.Validators
{
    public class AbastecimentoValidator : AbstractValidator<Abastecimento>
    {
        public AbastecimentoValidator()
        {
            RuleFor(x => x.VeiculoId)
                .GreaterThan(0)
                .WithMessage("O ID do veículo é obrigatório.");

            RuleFor(x => x.Litros)
                .GreaterThan(0)
                .WithMessage("A quantidade de litros deve ser superior a zero.");

            RuleFor(x => x.PrecoPorLitro)
                .GreaterThan(0)
                .WithMessage("O preço por litro deve ser superior a zero.");

            RuleFor(x => x.KmAtual)
                .GreaterThan(0)
                .WithMessage("O KM atual deve ser superior a zero.");

            RuleFor(x => x.Combustivel)
                .NotEmpty()
                .WithMessage("O tipo de combustível é obrigatório.");

            RuleFor(x => x.Posto)
                .NotEmpty()
                .WithMessage("O posto de abastecimento é obrigatório.");
        }
    }
}
