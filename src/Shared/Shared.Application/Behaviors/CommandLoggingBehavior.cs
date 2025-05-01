using Mediator;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Shared.Application.Behaviors;

public class CommandLoggingBehavior<TMessage, TResponse>(ILogger<CommandLoggingBehavior<TMessage, TResponse>> logger)
    : IPipelineBehavior<TMessage, TResponse>
    where TMessage : notnull, ICommand<TResponse>
    where TResponse : notnull
{
    public async ValueTask<TResponse> Handle(TMessage message, CancellationToken cancellationToken, MessageHandlerDelegate<TMessage, TResponse> next)
    {
        logger.LogInformation("[COMMAND START] Handle request={Request} - Response={Response} - RequestData={RequestData}",
            typeof(TMessage).Name, typeof(TResponse).Name, message);

        var timer = new Stopwatch();

        timer.Start();

        var response = await next(message, cancellationToken);

        timer.Stop();

        var timeTaken = timer.Elapsed;

        if (timeTaken.Seconds > 3)
        {
            logger.LogWarning("[PERFORMANCE] The request {Request} took {TimeTaken} seconds.",
                typeof(TMessage).Name, timeTaken.Seconds);
        }

        logger.LogInformation("[COMMAND END] Handled {Request} with {Response} - Elapsed time: {Elapsed}", typeof(TMessage).Name, typeof(TResponse).Name, timeTaken);

        return response;
    }
}

