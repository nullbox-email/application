using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.ValueObjects.ValueObject", Version = "1.0")]

namespace Nullbox.Security.Domain.Users;

public class EmailAddressValueObject : ValueObject
{
    public EmailAddressValueObject(string value, string normalizedValue)
    {
        Value = value;
        NormalizedValue = normalizedValue;
    }

    protected EmailAddressValueObject()
    {
        Value = null!;
        NormalizedValue = null!;
    }

    public string Value { get; private set; }
    public string NormalizedValue { get; private set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        // Using a yield return statement to return each element one at a time
        yield return Value;
        yield return NormalizedValue;
    }
}