using GestoreDeFrotas.Services; // Namespace correto conforme o teu projeto
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace GestoreDeFrotas.Filtro
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class AuditarAcaoFilter : Attribute, IAsyncActionFilter
    {
        private readonly AuditoriaService _auditoriaService;

        public AuditarAcaoFilter(AuditoriaService auditoriaService)
        {
            _auditoriaService = auditoriaService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();
            var httpContext = context.HttpContext;

            var utilizador = httpContext.User.Identity?.Name ?? "Utilizador Anónimo";
            var metodo = httpContext.Request.Method;
            var rota = httpContext.Request.Path;
            var acao = context.ActionDescriptor.DisplayName ?? "Ação Executada";

            if (resultContext.Exception != null)
            {
                string descricaoErro = $"Erro ao executar {acao}: {resultContext.Exception.Message}";
                await _auditoriaService.RegistarLogAsync(utilizador, metodo, rota, descricaoErro, 500);
            }
            else
            {
                int statusCode = httpContext.Response.StatusCode;
                string descricaoSucesso = $"Executou com sucesso: {acao}";
                await _auditoriaService.RegistarLogAsync(utilizador, metodo, rota, descricaoSucesso, statusCode);
            }
        }
    }
}