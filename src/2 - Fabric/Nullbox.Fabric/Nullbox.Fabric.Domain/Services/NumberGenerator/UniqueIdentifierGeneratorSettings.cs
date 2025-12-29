using System.Collections.Generic;
using System.Reflection.Metadata;

namespace Nullbox.Fabric.Domain.Services.NumberGenerator;

public class UniqueIdentifierGeneratorSettings
{
    public int Length { get; set; } = 8;
    public string Prefix { get; set; } = string.Empty;
    public string Postfix { get; set; } = string.Empty;
    public IEnumerable<char> Chars { get; set; } = "23456789ABCDEFGHJKMNPQRSTUVWXYZ";
}
