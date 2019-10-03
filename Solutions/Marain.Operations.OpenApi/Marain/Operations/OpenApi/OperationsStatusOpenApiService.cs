// <copyright file="OperationsStatusOpenApiService.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Operations.OpenApi
{
    using System;
    using System.Threading.Tasks;
    using Corvus.Tenancy;
    using Marain.Operations.Domain;
    using Marain.Operations.Tasks;
    using Menes;
    using Menes.Links;

    /// <summary>
    /// OpenApi service providing representations of long-running operations.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Services that implement long-running operations, and that wish to use this service to
    /// provide a representation of those operations to their clients, should use the
    /// <see cref="OperationsControlOpenApiService"/> to define and update the operations that this
    /// service presents.
    /// </para>
    /// </remarks>
    [EmbeddedOpenApiDefinition("Marain.Operations.OpenApi.OperationsStatus.yaml")]
    public class OperationsStatusOpenApiService : IOpenApiService
    {
        private readonly IOperationsStatusTasks tasks;
        private readonly IOpenApiWebLinkResolver linkResolver;
        private readonly ITenantProvider tenantProvider;

        /// <summary>
        /// Creates an <see cref="OperationsStatusOpenApiService"/>.
        /// </summary>
        /// <param name="tenantProvider">The tenant provider.</param>
        /// <param name="operationalStatusTasks">Underlying tasks.</param>
        /// <param name="linkResolver">The link resolver.</param>
        public OperationsStatusOpenApiService(
            ITenantProvider tenantProvider,
            IOperationsStatusTasks operationalStatusTasks,
            IOpenApiWebLinkResolver linkResolver)
        {
            this.tenantProvider = tenantProvider ?? throw new ArgumentNullException(nameof(tenantProvider));
            this.tasks = operationalStatusTasks ?? throw new ArgumentNullException(nameof(operationalStatusTasks));
            this.linkResolver = linkResolver ?? throw new ArgumentNullException(nameof(linkResolver));
        }

        /// <summary>
        /// Maps links for the service.
        /// </summary>
        /// <param name="links">The link operation map.</param>
        public static void MapLinks(IOpenApiLinkOperationMap links)
        {
            links.Map<Operation>("self", nameof(GetOperationById));
        }

        /// <summary>
        /// Gets the specified operation.
        /// </summary>
        /// <param name="tenantId">The tenant ID.</param>
        /// <param name="operationId">The operation's unique identifier.</param>
        /// <returns>A description of the HTTP response to produce.</returns>
        [OperationId(nameof(GetOperationById))]
        public async Task<OpenApiResult> GetOperationById(string tenantId, Guid operationId)
        {
            ITenant tenant = await this.DetermineTenantAsync(tenantId).ConfigureAwait(false);

            Operation operation = await this.tasks.GetAsync(tenant, operationId).ConfigureAwait(false);

            if (operation == null)
            {
                return this.NotFoundResult();
            }

            return IsCompleted(operation)
                ? this.OkResult(operation)
                : this.AcceptedResultWithHeader(operation);
        }

        private static bool IsCompleted(Operation operation)
        {
            return operation.Status == OperationStatus.Succeeded || operation.Status == OperationStatus.Failed;
        }

        private OpenApiResult AcceptedResultWithHeader(Operation operation)
        {
            WebLink link = this.linkResolver.Resolve(
                nameof(this.GetOperationById),
                "self",
                ("tenantId", operation.TenantId),
                ("operationId", operation.Id));
            OpenApiResult result = this.AcceptedResult(link.Href);
            result.Results.Add("application/json", operation);
            return result;
        }

        private Task<ITenant> DetermineTenantAsync(string tenantId)
        {
            // TODO: get the tenant from the context
            return this.tenantProvider.GetTenantAsync(tenantId);
        }
    }
}