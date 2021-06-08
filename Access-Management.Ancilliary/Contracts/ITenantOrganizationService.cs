namespace Access_Management.Ancilliary.Contracts
{
    public interface ITenantOrganizationService
    {
        int IdentifyTenant(string tenantDomain);
    }
}