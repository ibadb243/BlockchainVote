using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class DbInitializer
{
    public static void Initialize(params DbContext[] contexts)
    {
        foreach (var context in contexts)
        {
            context.Database.EnsureCreated();
        }
    }
}
