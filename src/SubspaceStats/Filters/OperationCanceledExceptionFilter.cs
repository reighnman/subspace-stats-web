using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SubspaceStats.Filters
{
    /// <summary>
    /// Filter that handles the <see cref="OperationCanceledException"/> for actions that are canceled.
    /// </summary>
    public class OperationCanceledExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<OperationCanceledExceptionFilter> _logger;

        public OperationCanceledExceptionFilter(ILogger<OperationCanceledExceptionFilter> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void OnException(ExceptionContext context)
        {
            if (context.Exception is OperationCanceledException)
            {
                _logger.LogInformation("Request was cancelled.");
                context.ExceptionHandled = true;
                context.Result = new StatusCodeResult(499); // client closed request
            }
        }
    }
}
