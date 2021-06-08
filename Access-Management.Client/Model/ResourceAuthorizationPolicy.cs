using System;

namespace Access_Management.Client.Model
{
    public class ResourceAuthorizationPolicy
    {
        public Guid Id { get; set; }

        public int TenantId { get; set; }

        public Guid? OrganizationId { get; set; }

        public AccessSource AccessSource { get; set; }

        public string ApplicationDomain { get; set; }
    }

    public enum AccessSource
    {
        API = 0, UserInterface = 1
    }
}