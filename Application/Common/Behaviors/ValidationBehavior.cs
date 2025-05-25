using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;

namespace Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, Result<TResponse>> 
    where TRequest : IRequest<Result<TResponse>>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public Task<Result<TResponse>> Handle(TRequest request, RequestHandlerDelegate<Result<TResponse>> next, CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);
        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(result => result.AsErrors())
            .Where(error => error != null)
            .ToList();

        if (failures.Any()) return Task.FromResult(Result<TResponse>.Invalid(failures));
        return next();
    }
}
