namespace Access_Management.Infrastructure.Model
{
    public static class Constants
    {
        /// <summary>
        /// Claim type for tenantId claim
        /// </summary>
        public const string TenantId = "http://schemas.microsoft.com/identity/claims/tenantid";

        /// <summary>
        /// Claim type for tenant domain claim
        /// </summary>

        public const string UniqueHomeEnvironmentIdentifier = "utid";

        public static class ErrorMessage
        {
            /// <summary>
            /// The duplicate tenant
            /// </summary>
            public const string DuplicateAuthorizationPolicy = "Authorization policy with same details already exists.";

            /// <summary>
            /// The new record created
            /// </summary>
            public const string NewRecordCreated = "New record created successfully.";

            /// <summary>
            /// The record deleted
            /// </summary>
            public const string RecordDeleted = "Record deleted successfully.";

            /// <summary>
            /// The access denied
            /// </summary>
            public const string AccessDenied = "Access Denied !!";

            /// <summary>
            /// The user not found
            /// </summary>
            public const string AuthroizationPolicyNotFound = "Authorization policy data could not be found or you do not have access.";

        }
    }
}