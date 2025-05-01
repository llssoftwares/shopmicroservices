using FluentValidation;
using Mediator;

namespace Shared.Application.Behaviors;

public class ValidationBehavior<TMessage, TResponse>(IEnumerable<IValidator<TMessage>> validators) 
    : IPipelineBehavior<TMessage, TResponse> where TMessage : ICommand<TResponse>
{
    public async ValueTask<TResponse> Handle(TMessage message, CancellationToken cancellationToken, MessageHandlerDelegate<TMessage, TResponse> next)
    {
        var context = new ValidationContext<TMessage>(message);

        var validationResults =
            await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures =
            validationResults
            .Where(r => r.Errors.Count != 0)
            .SelectMany(r => r.Errors)
            .ToList();

        return failures.Count != 0 
            ? throw new ValidationException(failures) 
            : await next(message, cancellationToken);
    }
}