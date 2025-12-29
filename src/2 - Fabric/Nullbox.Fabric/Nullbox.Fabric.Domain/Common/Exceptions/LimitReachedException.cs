using Intent.RoslynWeaver.Attributes;

namespace Nullbox.Fabric.Domain.Common.Exceptions;

public class LimitReachedException : Exception
{
    public LimitReachedException()
        : base() { }

    public LimitReachedException(string message)
        : base(message)
    {
    }

    public LimitReachedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}