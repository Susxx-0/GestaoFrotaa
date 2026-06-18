using FluentValidation;
using GestoreDeFrotas.Models;
using System;

namespace GestoreDeFrotas.Validators
{
    public class VeiculoValidator : AbstractValidator<Veiculo>
    {
        public VeiculoValidator()
        {
            RuleFor(v => v.Marca)
                .NotEmpty().WithMessage("A marca é obrigatória.")
                .MaximumLength(50).WithMessage("A marca não pode exceder 50 caracteres.");

            RuleFor(v => v.Modelo)
                .NotEmpty().WithMessage("O modelo é obrigatório.")
                .MaximumLength(50).WithMessage("O modelo não pode exceder 50 caracteres.");

            RuleFor(v => v.Matricula)
                .NotEmpty().WithMessage("A matrícula é obrigatória.")
                .MaximumLength(20).WithMessage("A matrícula não pode exceder 20 caracteres.");

            RuleFor(v => v.Ano)
                .InclusiveBetween(1900, DateTime.Now.Year)
                .WithMessage("Ano inválido.");

            RuleFor(v => v.Estado)
                .NotEmpty()
                .WithMessage("O estado é obrigatório.");

            RuleFor(v => v.CategoriaUsuario)
                .NotEmpty()
                .WithMessage("A categoria de utilizador é obrigatória.")
                .Must(c => c == "Empresa" || c == "Outros")
                .WithMessage("Categoria inválida. Use 'Empresa' ou 'Outros'.");
        }
    }
}
