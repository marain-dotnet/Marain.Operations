namespace Marain.Operations.Specs.Integration
{
    using System;
    using System.Threading.Tasks;
    using Corvus.Tenancy;

    internal class FakeTenantProvider : ITenantProvider
    {
        public FakeTenantProvider(RootTenant rootTenant)
        {
            this.Root = rootTenant;
        }

        public ITenant Root { get; }

        public Task<ITenant> CreateChildTenantAsync(string parentTenantId)
        {
            throw new InvalidOperationException("Tests should not hit this code path");
        }

        public Task DeleteTenantAsync(string tenantId)
        {
            throw new InvalidOperationException("Tests should not hit this code path");
        }

        public Task<TenantCollectionResult> GetChildrenAsync(string tenantId, int limit = 20, string? continuationToken = null)
        {
            throw new InvalidOperationException("Tests should not hit this code path");
        }

        public Task<ITenant> GetTenantAsync(string tenantId, string? eTag = null)
        {
            if (tenantId != RootTenant.RootTenantId)
            {
                throw new InvalidOperationException("Tests should not hit this code path");
            }

            return Task.FromResult(this.Root);
        }

        public Task<ITenant> UpdateTenantAsync(ITenant tenant)
        {
            throw new InvalidOperationException("Tests should not hit this code path");
        }
    }
}
