namespace Application.Common.Exceptions;

public class NotFoundException<TEntity> : Exception
{
    public NotFoundException()
        : base($"{typeof(TEntity).Name} not found!") { }
}
