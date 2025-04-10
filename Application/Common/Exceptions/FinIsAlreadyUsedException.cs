namespace Application.Common.Exceptions;

public class FinIsAlreadyUsedException : Exception
{
    public FinIsAlreadyUsedException(string fin)
        : base($"FIN {fin} is already used!") { }
}
