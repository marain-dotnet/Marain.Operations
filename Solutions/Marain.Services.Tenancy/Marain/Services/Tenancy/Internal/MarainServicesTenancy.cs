// <copyright file="MarainServicesTenancy.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Services.Tenancy.Internal
{
    using System;
    using System.Threading.Tasks;
    using Corvus.Tenancy;
    using Corvus.Tenancy.Exceptions;
    using Marain.TenantManagement;
    using Menes.Exceptions;

    /// <summary>
    /// Provides methods required by Marain services to validate and work with tenants.
    /// </summary>
    public class MarainServicesTenancy : IMarainServicesTenancy
    {
        private readonly ITenantProvider tenantProvider;
        private readonly MarainServiceConfiguration serviceConfiguration;

        /// <summary>
        /// Creates a new instance of the <see cref="MarainServicesTenancy"/> class.
        /// </summary>
        /// <param name="tenantProvider">The tenant management service.</param>
        /// <param name="serviceConfiguration">Service configuration for the current service.</param>
        public MarainServicesTenancy(
            ITenantProvider tenantProvider,
            MarainServiceConfiguration serviceConfiguration)
        {
            this.tenantProvider = tenantProvider;
            this.serviceConfiguration = serviceConfiguration;
        }

        /// <inheritdoc/>
        public async Task<ITenant> GetRequestingTenantAsync(string tenantId)
        {
            ITenant tenant = await this.GetTenantAsync(tenantId).ConfigureAwait(false);

            // Validate it's of the expected type. This will throw an ArgumentException if the tenant is not of the expected
            // type. This is not particularly useful, so we will catch this and instead throw an exception that will result
            // in a Not Found response.
            try
            {
                tenant.EnsureTenantIsOfType(MarainTenantType.Client, MarainTenantType.Delegated);
            }
            catch (ArgumentException)
            {
                throw new OpenApiNotFoundException($"The specified tenant Id, '{tenantId}', is of the wrong type for this request");
            }

            // Ensure the tenant is enrolled for the service.
            if (!tenant.IsEnrolledForService(this.serviceConfiguration.ServiceTenantId))
            {
                throw OpenApiForbiddenException.WithProblemDetails(
                    "Tenant not enrolled for service",
                    $"The tenant with Id '{tenantId}' is not enrolled in the service '{this.serviceConfiguration.ServiceDisplayName}' with Service Tenant Id '{this.serviceConfiguration.ServiceTenantId}'");
            }

            return tenant;
        }

        /// <inheritdoc/>
        public async Task<string> GetDelegatedTenantIdForRequestingTenantAsync(string tenantId)
        {
            ITenant tenant = await this.GetTenantAsync(tenantId).ConfigureAwait(false);
            return tenant.GetDelegatedTenantIdForServiceId(this.serviceConfiguration.ServiceTenantId);
        }

        private async Task<ITenant> GetTenantAsync(string tenantId)
        {
            // Get the incoming tenant from its Id. This will throw a TenantNotFoundException if the tenant doesn't exist.
            try
            {
                return await this.tenantProvider.GetTenantAsync(tenantId).ConfigureAwait(false);
            }
            catch (TenantNotFoundException)
            {
                throw new OpenApiNotFoundException($"No tenant matching the tenant Id '{tenantId}' can be found.");
            }
        }
    }
}
