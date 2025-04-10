namespace Application.Common.Exceptions;

public class YouAreNotPollCreatorException : Exception
{
    public YouAreNotPollCreatorException() : base("You are not Poll creator!") { }
}
