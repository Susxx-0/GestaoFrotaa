using FluentValidation;
using GestaoDeFrotas.Models;

namespace GestaoDeFrotas.Validators
{
    public class AbastecimentoValidator : AbstractValidator<Abastecimento>
    {
        public AbastecimentoValidator()
        {
            RuleFor(a => a.VeiculoId)
                .GreaterThan(0).WithMessage("O ID do veículo deve ser um identificador válido.");

            RuleFor(a => a.Litros)
                .InclusiveBetween(1, 200).WithMessage("Os litros abastecidos devem ser entre 1 e 200 litros.");

            RuleFor(a => a.PrecoPorLitro)
                .InclusiveBetween(0.1, 10.0).WithMessage("O preço por litro deve estar entre 0.1 e 10.0.");

            RuleFor(a => a.KmAtual)
                .InclusiveBetween(0, 1000000).WithMessage("Os quilómetros atuais devem ser entre 0 e 1.000.000 KM.");

            RuleFor(a => a.Combustivel)
                .NotEmpty().WithMessage("O tipo de combustível é obrigatório.")
                .MaximumLength(30).WithMessage("O tipo de combustível não pode ter mais de 30 caracteres.");

            RuleFor(a => a.Posto)
                .NotEmpty().WithMessage("O posto de combustível é obrigatório.")
                .MaximumLength(100).WithMessage("O nome do posto não pode ter mais de 100 caracteres.");

            RuleFor(a => a.Data)
                .LessThanOrEqualTo(DateTime.Now).WithMessage("A data do abastecimento não pode ser no futuro.");
        }
    }
}