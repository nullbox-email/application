using System.Diagnostics.CodeAnalysis;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Eventing.Contracts.AssemblyAttributes", Version = "1.0")]

[assembly: SuppressMessage("Formatting", "IDE0130:Namespace does not match folder structure.", Target = "Nullbox.Security.Users.Eventing.Messages.Users", Scope = "namespaceanddescendants", Justification = "Message namespaces need to be consistent between applications for deserialization to work")]
