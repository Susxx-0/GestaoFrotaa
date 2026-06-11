using FluentValidation;
using GestoreDeFrotas.Models;
using System;

namespace GestoreDeFrotas.Validators
{
    public class VeiculoValidator : AbstractValidator<Veiculo>
    {
        // CORREGIDO: Ahora es un constructor limpio, sin la palabra "class"
        public VeiculoValidator()
        {
            RuleFor(v => v.Marca)
                .NotEmpty().WithMessage("A marca do veículo é obrigatória.")
                .MaximumLength(50).WithMessage("A marca não pode ter mais de 50 caracteres.");

            RuleFor(v => v.Modelo)
                .NotEmpty().WithMessage("O modelo do veículo é obrigatório.")
                .MaximumLength(50).WithMessage("O modelo não pode ter mais de 50 caracteres.");

            // REMOVIDA A RESTRIÇÃO ESTRITA DE FORMATO (REGEX)
            RuleFor(v => v.Matricula)
                .NotEmpty().WithMessage("A matrícula é obrigatória.")
                .MinimumLength(4).WithMessage("A matrícula deve ter pelo menos 4 caracteres.")
                .MaximumLength(15).WithMessage("A matrícula não pode ter mais de 15 caracteres.");

            RuleFor(v => v.Ano)
                .InclusiveBetween(1900, DateTime.Now.Year).WithMessage($"O ano do veículo deve ser entre 1900 e {DateTime.Now.Year}.");

            RuleFor(v => v.Estado)
                .NotEmpty().WithMessage("O estado do veículo é obrigatório.");

            RuleFor(v => v.CategoriaUsuario)
                .NotEmpty().WithMessage("A categoria de utilizador é obrigatória.")
                .Must(c => c == "Empresa" || c == "Outros").WithMessage("A categoria deve ser 'Empresa' ou 'Outros'.");
        }
    }
}