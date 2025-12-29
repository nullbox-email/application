using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Azure.Provisioning.CosmosDB;
using Azure.Provisioning.KeyVault;
using Azure.Provisioning.Storage;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Aryzac.Aspire.Host.AppHost", Version = "1.0")]

var builder = DistributedApplication.CreateBuilder(args);

// Provision Azure Container Apps environment
// [IntentIgnore]
builder.AddAzureContainerAppEnvironment("env");

// Provision Application Insights resource
var insights = builder.AddAzureApplicationInsights("application-insights");
var applicationInsightsConnectionString = new ConnectionStringReference(insights.Resource, optional: false);

// Provision Azure Key Vault resource (shared)
var kv = builder.AddAzureKeyVault("shared-kv");

// Provision Cosmos Db resource
#pragma warning disable ASPIRECOSMOSDB001
var cosmos = builder.AddAzureCosmosDB("cosmos-db")
    // Store Cosmos access-key connection string in the shared Key Vault
    // [IntentIgnore]
    .WithAccessKeyAuthentication(kv)
    // [IntentIgnore]
    .ConfigureInfrastructure(infra =>
    {
        var cosmosDbAccount = infra.GetProvisionableResources()
                                   .OfType<CosmosDBAccount>()
                                   .Single();

        cosmosDbAccount.DisableLocalAuth = false;

        cosmosDbAccount.BackupPolicy = new ContinuousModeBackupPolicy()
        {
            ContinuousModeTier = ContinuousTier.Continuous7Days
        };
    })
    .RunAsPreviewEmulator(emulator =>
    {
        emulator.WithDataExplorer();
        emulator.WithLifetime(ContainerLifetime.Persistent);
        emulator.WithDataVolume();
    });
#pragma warning restore ASPIRECOSMOSDB001

// Provision Azure Storage resource
var azureStorage = builder.AddAzureStorage("storage")
    // [IntentIgnore]
    .ConfigureInfrastructure(infra =>
    {
        var storageAccount = infra.GetProvisionableResources()
                                  .OfType<StorageAccount>()
                                  .Single();

        storageAccount.AllowSharedKeyAccess = true;
    })
    .RunAsEmulator(azurite =>
    {
        azurite.WithLifetime(ContainerLifetime.Persistent);
        azurite.WithDataVolume();
    });

// Provision RabbitMQ resource
// [IntentIgnore]
var rabbitMq = builder.AddRabbitMQ("messaging").WithManagementPlugin();
var rabbitMqConnectionStringExpr = rabbitMq.Resource.ConnectionStringExpression;
// [IntentIgnore]
var rabbitMqHostString = rabbitMq.Resource.Host;
// [IntentIgnore]
var rabbitMqPortString = rabbitMq.Resource.Port;
// [IntentIgnore]
var rabbitMqPasswordString = rabbitMq.Resource.PasswordParameter;

// Parameters
// [IntentIgnore]
var versionParameter = builder.AddParameter("VERSION");
// [IntentIgnore]
var publicApiUrlParameter = builder.AddParameter("PUBLICAPIURL");
// [IntentIgnore]
var oauthEntraExternalClientIdParameter = builder.AddParameter("OAUTHENTRAEXTERNALCLIENTID");
// [IntentIgnore]
var oauthEntraExternalTenantParameter = builder.AddParameter("OAUTHENTRAEXTERNALTENANT");
// [IntentIgnore]
var oauthEntraExternalTenantIdParameter = builder.AddParameter("OAUTHENTRAEXTERNALTENANTID");
// [IntentIgnore]
var tokenExpirySkewMsParameter = builder.AddParameter("TOKENEXPIRYSKEWMS");
// [IntentIgnore]
var securityBearerIssuerSigningKeyParameter = builder.AddParameter("SECURITYBEARERISSUERSIGNINGKEY", secret: true);
// [IntentIgnore]
var securityBearerHmacKeyParameter = builder.AddParameter("SECURITYBEARERHMACKEY");
// [IntentIgnore]
var emailIngressHmacSecretParameter = builder.AddParameter("EMAILINGRESSHMACSECRET", secret: true);
// [IntentIgnore]
var nuxtSessionPasswordParameter = builder.AddParameter("NUXTSESSIONPASSWORD", secret: true);
// [IntentIgnore]
var tenantIdValue = await oauthEntraExternalTenantIdParameter.Resource.GetValueAsync(default);
// [IntentIgnore]
var oauthEntraExternalAuthorityParameter = $"https://{tenantIdValue}.ciamlogin.com/{tenantIdValue}/v2.0";

// Service Definitions
// [IntentIgnore]
var nullboxAuthEntraExternalIdApi = builder.AddProject<Projects.Nullbox_Auth_EntraExternalId_Api>("authentra-external-id");
// [IntentIgnore]
var nullboxSecurityApi = builder.AddProject<Projects.Nullbox_Security_Api>("security");
// [IntentIgnore]
var nullboxFabricApi = builder.AddProject<Projects.Nullbox_Fabric_Api>("fabric");

// Common Key Vault settings for your existing Program.cs loader
var keyVaultEnabled = builder.ExecutionContext.IsRunMode ? "false" : "true";
var keyVaultEndpointExpr = kv.Resource.UriExpression; // https://{name}.vault.azure.net/ 

// ---- Nullbox.Auth.EntraExternalId configuration ----
nullboxAuthEntraExternalIdApi
    // IMPORTANT: prevent “reference env splatting” from generating Cosmos output dependencies
    .WithReferenceEnvironment(ReferenceEnvironmentInjectionFlags.None)
    .WithEnvironment("ApplicationInsights__ConnectionString", applicationInsightsConnectionString)
    .WithReference(insights)
    // Enable your KeyVault loader in deploy mode
    .WithEnvironment("KeyVault__Enabled", keyVaultEnabled)
    .WithEnvironment("KeyVault__Endpoint", keyVaultEndpointExpr)
    // Give the app identity permission to list+read secrets (config provider needs both) :contentReference[oaicite:3]{index=3}
    .WithRoleAssignments(kv, KeyVaultBuiltInRole.KeyVaultReader, KeyVaultBuiltInRole.KeyVaultSecretsUser)
    .WithEnvironment("HttpClients__Nullbox.Security.Tokens.Services__Uri", nullboxSecurityApi.GetEndpoint("https"))
    .WithReference(nullboxSecurityApi)
    .WaitFor(nullboxSecurityApi)
    // [IntentIgnore]
    .WithEnvironment("Security-Bearer__Authority", oauthEntraExternalAuthorityParameter)
    // [IntentIgnore]
    .WithEnvironment("Security-Bearer__Audience", oauthEntraExternalClientIdParameter);

// ---- Nullbox.Security configuration ----
var nullboxSecurityDB = cosmos.AddCosmosDatabase("nullbox-security-db", "Nullbox.SecurityDB");

nullboxSecurityApi
    .WithReferenceEnvironment(ReferenceEnvironmentInjectionFlags.None)
    .WithEnvironment("ApplicationInsights__ConnectionString", applicationInsightsConnectionString)
    .WithReference(insights)
    // KeyVault loader settings
    .WithEnvironment("KeyVault__Enabled", keyVaultEnabled)
    .WithEnvironment("KeyVault__Endpoint", keyVaultEndpointExpr)
    .WithRoleAssignments(kv, KeyVaultBuiltInRole.KeyVaultReader, KeyVaultBuiltInRole.KeyVaultSecretsUser)
    // Token signing configuration
    .WithEnvironment("Security-Bearer__IssuerSigningKey", securityBearerIssuerSigningKeyParameter)
    .WithEnvironment("Security-Bearer__HmacKey", securityBearerHmacKeyParameter)
    // RabbitMQ (use your real env name)
    .WithEnvironment(name: "RabbitMq__ConnectionString", rabbitMqConnectionStringExpr)
    // [IntentIgnore]
    .WithEnvironment(name: "RabbitMq__Host", rabbitMqHostString)
    // [IntentIgnore]
    .WithEnvironment(name: "RabbitMq__Port", rabbitMqPortString)
    // [IntentIgnore]
    .WithEnvironment(name: "RabbitMq__Password", rabbitMqPasswordString)
    // [IntentIgnore]
    .WithReference(rabbitMq)
    // [IntentIgnore]
    .WaitFor(rabbitMq)
    // Ensure DB exists before start (local run ordering)
    .WaitFor(nullboxSecurityDB);

// ---- Nullbox.Fabric configuration ----
var nullboxFabricDB = cosmos.AddCosmosDatabase("nullbox-fabric-db", "Nullbox.FabricDB");

var nullboxFabricStorage = azureStorage.AddBlobs("blobs-nullbox-fabric");
var nullboxFabricStorageConnectionString = nullboxFabricStorage.Resource.ConnectionStringExpression;

nullboxFabricApi
    .WithReferenceEnvironment(ReferenceEnvironmentInjectionFlags.None)
    .WithEnvironment("ApplicationInsights__ConnectionString", applicationInsightsConnectionString)
    .WithReference(insights)
    // KeyVault loader settings
    .WithEnvironment("KeyVault__Enabled", keyVaultEnabled)
    .WithEnvironment("KeyVault__Endpoint", keyVaultEndpointExpr)
    .WithRoleAssignments(kv, KeyVaultBuiltInRole.KeyVaultReader, KeyVaultBuiltInRole.KeyVaultSecretsUser)
    // Signed email ingress
    .WithEnvironment("EmailIngress__HmacSecret", emailIngressHmacSecretParameter)
    // RabbitMQ (use your real env name)
    .WithEnvironment(name: "RabbitMq__ConnectionString", rabbitMqConnectionStringExpr)
    // [IntentIgnore]
    .WithEnvironment(name: "RabbitMq__Host", rabbitMqHostString)
    // [IntentIgnore]
    .WithEnvironment(name: "RabbitMq__Port", rabbitMqPortString)
    // [IntentIgnore]
    .WithEnvironment(name: "RabbitMq__Password", rabbitMqPasswordString)
    // [IntentIgnore]
    .WithReference(rabbitMq)
    // [IntentIgnore]
    .WaitFor(rabbitMq)
    // Storage (your existing correct pattern)
    .WithEnvironment("AzureBlobStorage", nullboxFabricStorageConnectionString)
    .WithReference(nullboxFabricStorage)
    .WaitFor(nullboxFabricStorage)
    // Ensure DB exists before start (local run ordering)
    .WaitFor(nullboxFabricDB)
    // [IntentIgnore]
    .WithEnvironment("Security-Bearer__Authority", nullboxSecurityApi.GetEndpoint("https"));

// LOCAL RUN ONLY: feed emulator connstring via ConnectionStrings__cosmos-db so your code can use GetConnectionString("cosmos-db")
if (builder.ExecutionContext.IsRunMode)
{
    nullboxSecurityApi.WithEnvironment("ConnectionStrings__cosmos-db", cosmos.Resource.ConnectionStringExpression);
    nullboxFabricApi.WithEnvironment("ConnectionStrings__cosmos-db", cosmos.Resource.ConnectionStringExpression);
}

// Application Gateway (YARP) configuration
var gateway = builder.AddYarp("gateway");
// [IntentIgnore]
gateway.WithReference(nullboxAuthEntraExternalIdApi).WaitFor(nullboxAuthEntraExternalIdApi);
// [IntentIgnore]
gateway.WithReference(nullboxSecurityApi).WaitFor(nullboxSecurityApi);
// [IntentIgnore]
gateway.WithReference(nullboxFabricApi).WaitFor(nullboxFabricApi);
gateway.WithConfiguration(yarp =>
{
    var nullboxFabricCluster = yarp.AddCluster(resource: nullboxFabricApi);
    yarp.AddRoute("/{version}/mailboxes/{mailboxId}/aliases", nullboxFabricCluster);
    yarp.AddRoute("/{version}/mailboxes/{mailboxId}/aliases/{id}", nullboxFabricCluster);
    yarp.AddRoute("/{version}/dashboard", nullboxFabricCluster);
    yarp.AddRoute("/{version}/email/{deliveryActionId}/complete", nullboxFabricCluster);
    yarp.AddRoute("/{version}/email/{deliveryActionId}/quarantine", nullboxFabricCluster);
    yarp.AddRoute("/{version}/email", nullboxFabricCluster);
    yarp.AddRoute("/{version}/mailboxes", nullboxFabricCluster);
    yarp.AddRoute("/{version}/mailboxes/{id}", nullboxFabricCluster);

    var nullboxSecurityCluster = yarp.AddCluster(resource: nullboxSecurityApi);
    yarp.AddRoute("/{version}/tokens/access", nullboxSecurityCluster);
    yarp.AddRoute("/{version}/on-board/user", nullboxSecurityCluster);

    var nullboxAuthEntraExternalIdCluster = yarp.AddCluster(resource: nullboxAuthEntraExternalIdApi);
    yarp.AddRoute("/{version}/tokens/exchange", nullboxAuthEntraExternalIdCluster);
});
// [IntentIgnore]
gateway.WithExternalHttpEndpoints();

// Nuxt Application configuration
// [IntentIgnore]
var application = builder.AddViteApp("application", "../../9 - UI/application")
    .WithWorkingDirectory("../../9 - UI/application")
    .WithReference(gateway)
    .WaitFor(gateway)
    .WithEnvironment("NUXT_PUBLIC_VERSION", versionParameter)
    .WithEnvironment("NUXT_PUBLIC_ENTRA_EXTERNAL_TENANT", oauthEntraExternalTenantParameter)
    .WithEnvironment("NUXT_PUBLIC_ENTRA_EXTERNAL_TENANT_ID", oauthEntraExternalTenantIdParameter)
    .WithEnvironment("NUXT_SESSION_PASSWORD", nuxtSessionPasswordParameter)
    .WithEnvironment("NUXT_OAUTH_ENTRAEXTERNAL_CLIENT_ID", oauthEntraExternalClientIdParameter)
    .WithEnvironment("NUXT_OAUTH_ENTRAEXTERNAL_TENANT", oauthEntraExternalTenantParameter)
    .WithEnvironment("NUXT_OAUTH_ENTRAEXTERNAL_TENANT_ID", oauthEntraExternalTenantIdParameter)
    .WithEnvironment("NUXT_TOKEN_EXPIRY_SKEW_MS", tokenExpirySkewMsParameter)
    .PublishAsDockerFile()
    .WithExternalHttpEndpoints();

// Nuxt Web configuration
// [IntentIgnore]
var web = builder.AddViteApp("web", "../../9 - UI/web")
    .WithWorkingDirectory("../../9 - UI/web")
    .WithEnvironment("NUXT_PUBLIC_VERSION", versionParameter)
    .WithEnvironment("NUXT_SESSION_PASSWORD", nuxtSessionPasswordParameter)
    .PublishAsDockerFile()
    .WithExternalHttpEndpoints();

// Nuxt Docs configuration
// [IntentIgnore]
var docs = builder.AddViteApp("docs", "../../9 - UI/docs")
    .WithWorkingDirectory("../../9 - UI/docs")
    .WithEnvironment("NUXT_PUBLIC_VERSION", versionParameter)
    .PublishAsDockerFile()
    .WithExternalHttpEndpoints();

// [IntentIgnore]
if (builder.ExecutionContext.IsRunMode)
{
    application
        .WithEnvironment("NUXT_PUBLIC_API_URL", gateway.GetEndpoint("https"))
        .WithEndpoint("http", (endpointAnnotation) =>
        {
            endpointAnnotation.Port = 3000;
        });

    var tunnel = builder.AddDevTunnel("gateway-tunnel")
        .WithReference(gateway)
        .WithAnonymousAccess();
}
else
{
    application
        .WithEnvironment("NUXT_PUBLIC_API_URL", publicApiUrlParameter)
        .PublishAsDockerFile();
}

builder.Build().Run();
