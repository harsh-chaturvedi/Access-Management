namespace Access_Management.Model
{
    public static class Constants
    {        
        /// <summary>
        /// </summary>
        public static class ClaimType
        {
            /// <summary>
            /// The role
            /// </summary>
            public const string Role = "Application.Identity.Role"; // Prepending Application.Identity to ignore contradiction with ADFS claims.
        }

        /// <summary>
        /// </summary>
        public static class ClaimValue
        {
            /// <summary>
            /// The admin
            /// </summary>
            public const string Admin = "Administrator";

            /// <summary>
            /// The super admin
            /// </summary>
            public const string SuperAdmin = "Super Administrator";
        }
    }
}