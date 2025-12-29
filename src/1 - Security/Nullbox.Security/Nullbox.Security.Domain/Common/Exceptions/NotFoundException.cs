using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.NotFoundException", Version = "1.0")]

namespace Nullbox.Security.Domain.Common.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException()
    {
    }

    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}