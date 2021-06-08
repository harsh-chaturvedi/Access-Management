using System;
using System.Collections.Generic;

namespace Access_Management.Infrastructure.Model
{
    public class ResourceAuthorizationPolicy
    {
        public Guid Id { get; set; }

        public int TenantId { get; set; }

        public Guid? OrganizationId { get; set; }

        public AccessSource AccessSource { get; set; }

        public string ApplicationDomain { get; set; }

        public virtual ICollection<ResourceAuthorizationRule> AuthorizationRules { get; set; }
    }
}