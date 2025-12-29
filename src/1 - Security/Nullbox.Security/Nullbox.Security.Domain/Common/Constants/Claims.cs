namespace Nullbox.Security.Domain.Common.Constants;

public static class Claims
{
    public const string ClaimPrefix = "https://nullbox.email/claims";

    public static class User
    {
        public const string Status = "user/status";
    }
    public static class Permissions
    {
        public const string RestrictedAccess = "permission/restricted_access";
    }

    public static string GetClaimType(string claimType)
    {
        return $"{ClaimPrefix}/{claimType}";
    }
}