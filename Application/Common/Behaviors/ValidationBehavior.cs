using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;

    public ValidationBehavior(
        IEnumerable<IValidator<TRequest>> validators,
        ILogger<ValidationBehavior<TRequest, TResponse>> logger)
    {
        _validators = validators;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"ValidationBehavior called for {typeof(TRequest).Name}");
        _logger.LogInformation($"Found {_validators.Count()} validators");

        if (!_validators.Any())
        {
            _logger.LogWarning($"No validators found for {typeof(TRequest).Name}");
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v =>
            {
                _logger.LogInformation($"Running validator: {v.GetType().Name}");
                return v.ValidateAsync(context, cancellationToken);
            })
        );

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .Select(f => new ValidationError(f.PropertyName, f.ErrorMessage, f.ErrorCode, ValidationSeverity.Error))
            .ToList();

        _logger.LogInformation($"Found {failures.Count} validation errors");

        if (failures.Any())
        {
            foreach (var failure in failures)
            {
                _logger.LogError($"Validation error: {failure.Identifier} - {failure.ErrorMessage}");
            }

            if (IsResultType(typeof(TResponse)))
                return CreateInvalidResult<TResponse>(failures);
            else
                throw new ValidationException(failures[0].ErrorMessage);
        }

        return await next();
    }

    private static bool IsResultType(Type type)
    {
        if (!type.IsGenericType)
            return false;

        var genericTypeDefinition = type.GetGenericTypeDefinition();
        return genericTypeDefinition == typeof(Result<>) || genericTypeDefinition == typeof(Result);
    }

    private static TResponse CreateInvalidResult<T>(List<ValidationError> validationErrors)
    {
        var resultType = typeof(T);

        if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var valueType = resultType.GetGenericArguments()[0];
            var invalidMethod = typeof(Result<>).MakeGenericType(valueType)
                .GetMethod("Invalid", new[] { typeof(ValidationError[]) });

            return (TResponse)invalidMethod!.Invoke(null, new object[] { validationErrors.ToArray() });
        }

        if (resultType == typeof(Result))
        {
            return (TResponse)(object)Result.Invalid(validationErrors.ToArray());
        }

        throw new InvalidOperationException($"Cannot create invalid result for type {resultType.Name}");
    }
}
