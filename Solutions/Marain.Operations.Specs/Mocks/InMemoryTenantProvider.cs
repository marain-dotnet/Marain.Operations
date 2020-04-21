// <copyright file="InMemoryTenantProvider.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Specs.Mocks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Corvus.Extensions.Json;
    using Corvus.Tenancy;
    using Corvus.Tenancy.Exceptions;
    using Newtonsoft.Json;

    /// <summary>
    /// In-memory implementation of ITenantProvider.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Tenants are stored in internal List{T} and Dictionary{T, T}; as such, this implementation should not be considered
    /// thread-safe.
    /// </para>
    /// <para>
    /// In order to emulate the behaviour of tenant providers that store their data in an external store (or the
    /// <c>ClientTenantProvider</c>, which internally calls the tenancy REST API), data is stored internally in serialized
    /// form. When <see cref="ITenant"/> instances are requested (e.g. via <see cref="GetTenantAsync(string, string)"/>, the
    /// appropriate data is retrieved and deserialized to an <see cref="ITenant"/> before being returned. This means that
    /// multiple calls to a method such as <see cref="GetTenantAsync(string, string)"/> will return different object instances.
    /// This is done to avoid any potential issues with tests passing when using the <see cref="InMemoryTenantProvider"/> but
    /// failing when switching to a real implementation. For example, a possible cause of this would be a spec testing an
    /// operation that updates a tenant and later verifying that those changes have been made. If we simply stored the tenant
    /// in memory, it would be possible for the code under test to omit calling
    /// <see cref="UpdateTenantAsync(ITenant)"/>, but for the test which checks that the updates have been made to still
    /// succeed. By ensuring that a unique <see cref="ITenant"/> is returned each time, we avoid this and similar problems.
    /// </para>
    /// </remarks>
    public class InMemoryTenantProvider : ITenantProvider
    {
        private readonly IJsonSerializerSettingsProvider jsonSerializerSettingsProvider;
        private readonly List<StoredTenant> allTenants = new List<StoredTenant>();
        private readonly Dictionary<string, List<string>> tenantsByParent = new Dictionary<string, List<string>>();

        public InMemoryTenantProvider(RootTenant rootTenant, IJsonSerializerSettingsProvider jsonSerializerSettingsProvider)
        {
            this.Root = rootTenant;
            this.jsonSerializerSettingsProvider = jsonSerializerSettingsProvider;
        }

        public ITenant Root { get; }

        public Task<ITenant> CreateChildTenantAsync(string parentTenantId, string name)
        {
            return this.CreateWellKnownChildTenantAsync(parentTenantId, Guid.NewGuid(), name);
        }

        public async Task<ITenant> CreateWellKnownChildTenantAsync(string parentTenantId, Guid wellKnownChildTenantGuid, string name)
        {
            ITenant parent = await this.GetTenantAsync(parentTenantId).ConfigureAwait(false);
            var newTenant = new Tenant(this.jsonSerializerSettingsProvider)
            {
                Id = parent.Id.CreateChildId(wellKnownChildTenantGuid),
                Name = name,
            };

            List<string> childrenList = this.GetChildren(parent.Id);
            childrenList.Add(newTenant.Id);
            this.allTenants.Add(new StoredTenant(newTenant, this.jsonSerializerSettingsProvider.Instance));

            return newTenant;
        }

        public Task DeleteTenantAsync(string tenantId)
        {
            if (this.tenantsByParent.TryGetValue(tenantId, out List<string>? children) && children.Count > 0)
            {
                throw new InvalidOperationException("Cannot delete a tenant with children.");
            }

            StoredTenant storedTenant = this.allTenants.Single(x => x.Id == tenantId);
            this.allTenants.Remove(storedTenant);

            string parentTenantId = storedTenant.Tenant.GetRequiredParentId();

            List<string> siblings = this.tenantsByParent[parentTenantId];
            siblings.Remove(tenantId);

            return Task.CompletedTask;
        }

        public async Task<TenantCollectionResult> GetChildrenAsync(string tenantId, int limit = 20, string? continuationToken = null)
        {
            ITenant parent = await this.GetTenantAsync(tenantId).ConfigureAwait(false);

            List<string> children = this.GetChildren(parent.Id);

            int skip = 0;

            if (!string.IsNullOrEmpty(continuationToken))
            {
                skip = int.Parse(continuationToken);
            }

            IEnumerable<string> tenants = children.Skip(skip).Take(limit);

            continuationToken = tenants.Count() == limit ? (skip + limit).ToString() : null;

            return new TenantCollectionResult(tenants, continuationToken);
        }

        public Task<ITenant> GetTenantAsync(string tenantId, string? eTag = null)
        {
            ITenant? tenant = tenantId == this.Root.Id
                ? this.Root
                : this.allTenants.Find(x => x.Id == tenantId)?.Tenant;

            if (tenant == null)
            {
                throw new TenantNotFoundException();
            }

            return Task.FromResult(tenant);
        }

        public Task<ITenant> UpdateTenantAsync(ITenant tenant)
        {
            StoredTenant? currentStoredTenant = this.allTenants.Find(x => x.Id == tenant.Id);

            if (currentStoredTenant == null)
            {
                throw new TenantNotFoundException($"Cannot update tenant '{tenant.Name}' with Id '{tenant.Id}' because it has not previously been saved.");
            }

            currentStoredTenant.Tenant = tenant;

            return Task.FromResult(tenant);
        }

        public ITenant? GetTenantByName(string name)
        {
            return this.allTenants.Find(x => x.Name == name)?.Tenant;
        }

        public List<string> GetChildren(string parentId)
        {
            if (!this.tenantsByParent.TryGetValue(parentId, out List<string>? children))
            {
                children = new List<string>();
                this.tenantsByParent.Add(parentId, children);
            }

            return children;
        }

        /// <summary>
        /// Helper class to represent a tenant being held in memory. Tenant data is held in serialized JSON form. The reasons
        /// behind storing tenants in this form are explained in the <c>remarks</c> section of the documentation for the
        /// <see cref="InMemoryTenantProvider"/> class.
        /// </summary>
        private class StoredTenant
        {
            private readonly JsonSerializerSettings settings;
            private string tenant = string.Empty;

            public StoredTenant(ITenant tenant, JsonSerializerSettings settings)
            {
                this.settings = settings;
                this.Id = tenant.Id;
                this.Name = tenant.Name;
                this.Tenant = tenant;
            }

            public string Id { get; }

            public string Name { get; }

            public ITenant Tenant
            {
                get => JsonConvert.DeserializeObject<Tenant>(this.tenant, this.settings);
                set => this.tenant = JsonConvert.SerializeObject(value, this.settings);
            }
        }
    }
}
