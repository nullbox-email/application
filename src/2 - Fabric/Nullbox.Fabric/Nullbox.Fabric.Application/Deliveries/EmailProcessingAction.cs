using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Application.Dtos.ContractEnumModel", Version = "1.0")]

namespace Nullbox.Fabric.Application.Deliveries;

public enum EmailProcessingAction
{
    Drop,
    Forward,
    Quarantine
}