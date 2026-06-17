using FluentValidation;
using GestoreDeFrotas.Models;
using System;

namespace GestoreDeFrotas.Validators
{
    public class VeiculoValidator : AbstractValidator<Veiculo>
    {
        public VeiculoValidator()
        {
            RuleFor(v => v.Marca).NotEmpty().WithMessage("A marca é obrigatória.").MaximumLength(50);
            RuleFor(v => v.Modelo).NotEmpty().WithMessage("O modelo é obrigatório.").MaximumLength(50);
            RuleFor(v => v.Matricula).NotEmpty().WithMessage("A matrícula é obrigatória.");
            RuleFor(v => v.Ano).InclusiveBetween(1900, DateTime.Now.Year).WithMessage("Ano inválido.");
            RuleFor(v => v.Estado).NotEmpty().WithMessage("O estado é obrigatório.");
            RuleFor(v => v.CategoriaUsuario).NotEmpty().Must(c => c == "Empresa" || c == "Outros").WithMessage("Categoria inválida.");
        }
    }
}