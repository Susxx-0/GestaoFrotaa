using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;

namespace GestoreDeFrotas.Validators
{
    public class DocumentoUploadDto
    {
        public int VeiculoId { get; set; }
        public string TipoDocumento { get; set; } = string.Empty;
        public DateTime? DataValidade { get; set; }
        public IFormFile? Ficheiro { get; set; }
    }

    public class DocumentoUploadValidator : AbstractValidator<DocumentoUploadDto>
    {
        public DocumentoUploadValidator()
        {
            RuleFor(d => d.VeiculoId).GreaterThan(0);
            RuleFor(d => d.TipoDocumento).NotEmpty().MaximumLength(100);
            RuleFor(d => d.Ficheiro).NotNull().Must(f => f == null || f.Length > 0).WithMessage("Ficheiro inválido.");
        }
    }
}