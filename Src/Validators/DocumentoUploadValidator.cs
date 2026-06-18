using FluentValidation;
using GestoreDeFrotas.Models;

namespace GestoreDeFrotas.Validators
{
    public class DocumentoUploadValidator : AbstractValidator<DocumentoUploadDto>
    {
        public DocumentoUploadValidator()
        {
            RuleFor(x => x.VeiculoId)
                .GreaterThan(0)
                .WithMessage("O ID do veículo é obrigatório.");

            RuleFor(x => x.TipoDocumento)
                .NotEmpty()
                .WithMessage("O tipo de documento é obrigatório.");

            RuleFor(x => x.Ficheiro)
                .NotNull()
                .WithMessage("É necessário enviar um ficheiro.");

            RuleFor(x => x.Ficheiro!.Length)
                .LessThanOrEqualTo(10 * 1024 * 1024)
                .WithMessage("O ficheiro não pode exceder 10 MB.");
        }
    }
}
