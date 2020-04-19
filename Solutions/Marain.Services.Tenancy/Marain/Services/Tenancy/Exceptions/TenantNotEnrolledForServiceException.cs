// <copyright file="TenantNotEnrolledForServiceException.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Services.Tenancy.Exceptions
{
    using System;
    using Menes;

    /// <summary>
    /// An exception which is thrown from <see cref="IMarainServiceTenancyHelper.GetRequestingTenantAsync(string)"/>
    /// when the requesting tenant is not enrolled for the current service.
    /// </summary>
    [Serializable]
    public class TenantNotEnrolledForServiceException : Exception
    {
        /// <summary>
        /// Creates a new instance of the exception.
        /// </summary>
        /// <param name="requestingTenantId">The tenant Id supplied with the incoming request.</param>
        /// <param name="serviceTenantId">The Id of the service tenant for the Marain service.</param>
        /// <param name="serviceDisplayName">The display name for the Marain service.</param>
        public TenantNotEnrolledForServiceException(
            string requestingTenantId,
            string serviceTenantId,
            string serviceDisplayName)
            : base($"The requesting tenant with Id '{requestingTenantId}' is not enrolled in the service '{serviceDisplayName}' with Service Tenant Id '{serviceTenantId}'")
        {
            this.AddProblemDetailsTitle("Tenant not enrolled for use with service");
            this.AddProblemDetailsExplanation($"The requesting tenant with Id '{requestingTenantId}' is not enrolled in the service '{serviceDisplayName}'.");
            this.AddProblemDetailsExtension("requestingTenantId", requestingTenantId);
            this.AddProblemDetailsExtension("serviceDisplayName", serviceDisplayName);
        }
    }
}
