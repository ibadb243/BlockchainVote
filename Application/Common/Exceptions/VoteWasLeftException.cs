namespace Application.Common.Exceptions;

public class VoteWasLeftException : Exception
{
    public VoteWasLeftException() 
        : base("You have already voted!")
    {
        
    }
}
