namespace Marain.Operations.Specs.Integration
{
    using System;
    using System.Threading.Tasks;
    using Corvus.Tenancy;

    class FakeTenantProvider : ITenantProvider
    {
        public ITenant Root { get; } = new Tenant { Id = "Root" };

        public Task<ITenant> CreateChildTenantAsync(string parentTenantId)
        {
            throw new NotImplementedException();
        }

        public Task<TenantCollectionResult> GetChildrenAsync(string tenantId, int limit = 20, string continuationToken = null)
        {
            throw new NotImplementedException();
        }

        public Task<ITenant> GetTenantAsync(string tenantId)
        {
            throw new NotImplementedException();
        }

        public Task<ITenant> UpdateTenantAsync(ITenant tenant)
        {
            throw new NotImplementedException();
        }
    }
}
