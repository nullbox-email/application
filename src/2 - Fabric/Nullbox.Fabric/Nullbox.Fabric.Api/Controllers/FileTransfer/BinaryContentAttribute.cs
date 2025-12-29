using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.AspNetCore.Controllers.BinaryContentAttribute", Version = "1.0")]

namespace Nullbox.Fabric.Api.Controllers.FileTransfer;

[AttributeUsage(AttributeTargets.Method)]
public class BinaryContentAttribute : Attribute
{
}