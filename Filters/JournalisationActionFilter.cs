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
            var ctrl = context.RouteData.Values["controller"];
            var action = context.RouteData.Values["action"];
            _logger.LogInformation($"Action démarrée : {ctrl}/{action}");
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception != null)
            {
                var ctrl = context.RouteData.Values["controller"];
                var action = context.RouteData.Values["action"];
                _logger.LogError($"Erreur dans {ctrl}/{action} : {context.Exception.Message}");
            }
        }
    }
}
