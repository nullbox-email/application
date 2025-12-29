using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Deliveries;

[assembly: IntentTemplate("Intent.ValueObjects.ValueObject", Version = "1.0")]

namespace Nullbox.Fabric.Domain.ValueObjects;

public class QuarantineReasonValueObject : ValueObject
{
    public QuarantineReasonValueObject(QuarantineReason reason, string? message)
    {
        Reason = reason;
        Message = message;
    }

    protected QuarantineReasonValueObject()
    {
    }

    public QuarantineReason Reason { get; private set; }
    public string? Message { get; private set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        // Using a yield return statement to return each element one at a time
        yield return Reason;
        yield return Message;
    }
}