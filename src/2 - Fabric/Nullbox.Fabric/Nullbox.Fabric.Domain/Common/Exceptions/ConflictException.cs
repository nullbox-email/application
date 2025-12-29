using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.NotFoundException", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Common.Exceptions;

public class ConflictException : Exception
{
    public ConflictException()
    {
    }

    public ConflictException(string message) : base(message)
    {
    }

    public ConflictException(string message, Exception innerException) : base(message, innerException)
    {
    }
}