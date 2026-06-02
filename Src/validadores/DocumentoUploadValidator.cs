using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;

namespace GestaoDeFrotas.Validators
{
    // Classe auxiliar para mapear os dados do formulário
    public class DocumentoUploadDto
    {
        public int VeiculoId { get; set; }
        public string TipoDocumento { get; set; } = string.Empty;
        public DateTime? DataValidade { get; set; }
        public IFormFile? Ficheiro { get; set; }
    }

    // Regras de validação do FluentValidation
    public class DocumentoUploadValidator : AbstractValidator<DocumentoUploadDto>
    {
        public DocumentoUploadValidator()
        {
            RuleFor(d => d.VeiculoId)
                .GreaterThan(0).WithMessage("O ID do veículo deve ser um identificador válido.");

            RuleFor(d => d.TipoDocumento)
                .NotEmpty().WithMessage("O tipo de documento é obrigatório.")
                .MaximumLength(100).WithMessage("O tipo de documento não pode ultrapassar os 100 caracteres.");

            RuleFor(d => d.Ficheiro)
                .NotNull().WithMessage("Nenhum ficheiro foi enviado.")
                .Must(f => f == null || f.Length > 0).WithMessage("O ficheiro enviado está vazio.");
        }
    }
}