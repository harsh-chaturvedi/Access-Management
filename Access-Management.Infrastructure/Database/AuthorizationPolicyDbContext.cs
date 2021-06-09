using Access_Management.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Access_Management.Infrastructure.Database
{
    public class AuthorizationPolicyDbContext : DbContext
    {
        public AuthorizationPolicyDbContext(DbContextOptions<AuthorizationPolicyDbContext> options) : base(options)
        {

        }
        public DbSet<ResourceAuthorizationPolicy> AuthorizationPolicies { get; set; }
        public DbSet<ResourceAuthorizationRule> AuthorizationRules { get; set; }
    }
}