namespace Application.Common.Exceptions;

public class PollClosedException :  Exception
{
    public PollClosedException()
        : base("The Poll is already closed!") { }
}
