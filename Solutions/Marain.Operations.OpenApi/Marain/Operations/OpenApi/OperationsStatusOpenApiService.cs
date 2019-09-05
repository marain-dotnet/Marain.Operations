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
        private readonly ITenantProvider tenantProvider;

        /// <summary>
        /// Creates an <see cref="OperationsStatusOpenApiService"/>.
        /// </summary>
        /// <param name="tenantProvider">The tenant provider.</param>
        /// <param name="operationalStatusTasks">Underlying tasks.</param>
        public OperationsStatusOpenApiService(
            ITenantProvider tenantProvider,
            IOperationsStatusTasks operationalStatusTasks)
        {
            this.tenantProvider = tenantProvider;
            this.tasks = operationalStatusTasks;
        }

        /// <summary>
        /// Gets the specified operation.
        /// </summary>
        /// <param name="operationId">The operation's unique identifier.</param>
        /// <returns>A description of the HTTP response to produce.</returns>
        [OperationId(nameof(GetOperationById))]
        public async Task<OpenApiResult> GetOperationById(Guid operationId)
        {
            Operation operation = await this.tasks.GetAsync(this.DetermineTenant(), operationId).ConfigureAwait(false);

            return operation == null
                ? this.NotFoundResult()
                : this.OkResult(operation);
        }

        private ITenant DetermineTenant()
        {
            // TODO: determine tenant from the context
            return this.tenantProvider.Root;
        }
    }
}