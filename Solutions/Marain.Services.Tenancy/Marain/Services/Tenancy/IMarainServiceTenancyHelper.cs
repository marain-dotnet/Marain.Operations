﻿// <copyright file="IMarainServiceTenancyHelper.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Services.Tenancy
{
    using System.Threading.Tasks;
    using Corvus.Tenancy;
    using Corvus.Tenancy.Exceptions;
    using Marain.Services.Tenancy.Exceptions;
    using Menes.Exceptions;

    /// <summary>
    /// Provides methods required by Marain services to validate and work with tenants.
    /// </summary>
    public interface IMarainServiceTenancyHelper
    {
        /// <summary>
        /// Retrieves and validates the tenant matching the Id that was supplied with an incoming service request.
        /// </summary>
        /// <param name="tenantId">The tenant Id supplied with the request.</param>
        /// <returns>The tenant.</returns>
        /// <exception cref="TenantNotFoundException">The tenant Id is invalid.</exception>
        /// <exception cref="OpenApiBadRequestException">The tenant Id does not match a tenant of the correct type.</exception>
        /// <exception cref="TenantNotEnrolledForServiceException">The specified tenant is not enrolled for the service.</exception>
        Task<ITenant> GetRequestingTenantAsync(string tenantId);

        /// <summary>
        /// Retrieves the Id that this service should use when making requests on behalf of the specified tenant.
        /// </summary>
        /// <param name="tenantId">The tenant Id supplied with the request.</param>
        /// <returns>The Id of the delegated tenant that should be used to make further service calls.</returns>
        Task<string> GetDelegatedTenantIdForRequestingTenantAsync(string tenantId);
    }
}