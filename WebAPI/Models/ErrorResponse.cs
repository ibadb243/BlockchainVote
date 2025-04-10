namespace WebAPI.Models;

public class ErrorResponse
{
    public int status { get; set; }
    public string message { get; set; }
    public Dictionary<string, string> errors { get; set; }
}
