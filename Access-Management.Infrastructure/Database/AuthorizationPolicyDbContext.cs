using Access_Management.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;

namespace Access_Management.Infrastructure.Database
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<ResourceAuthorizationPolicy> AuthorizationPolicies { get; set; }
        public DbSet<ResourceAuthorizationRule> AuthorizationRules { get; set; }
    }
}