using Microsoft.AspNetCore.Mvc.Filters;

namespace GRH_SENTECH.Filters
{
    public class JournalisationActionFilter : IActionFilter
    {
        private readonly ILogger<JournalisationActionFilter> _logger;

        public JournalisationActionFilter(ILogger<JournalisationActionFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var controleur = context.RouteData.Values["controller"];
            var action = context.RouteData.Values["action"];
            _logger.LogInformation($"[GRH] Début : {controleur}/{action} — {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            var controleur = context.RouteData.Values["controller"];
            var action = context.RouteData.Values["action"];
            if (context.Exception != null)
                _logger.LogError($"[GRH] Erreur dans {controleur}/{action} : {context.Exception.Message}");
            else
                _logger.LogInformation($"[GRH] Fin : {controleur}/{action} — {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
        }
    }
}
