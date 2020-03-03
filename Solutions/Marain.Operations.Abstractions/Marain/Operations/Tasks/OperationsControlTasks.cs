// <copyright file="OperationsControlTasks.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Operations.Tasks
{
    using System;
    using System.Threading.Tasks;
    using Corvus.Tenancy;
    using Marain.Operations.Domain;
    using Marain.Operations.Storage;

    /// <summary>
    /// The underlying operations for getting and modifying the state of long running operations.
    /// </summary>
    public class OperationsControlTasks : IOperationsControlTasks
    {
        private readonly IOperationsRepository operationRepository;

        /// <summary>
        /// Creates a new <see cref="OperationsControlTasks"/>.
        /// </summary>
        /// <param name="operationRepository">Storage.</param>
        public OperationsControlTasks(IOperationsRepository operationRepository)
        {
            this.operationRepository = operationRepository;
        }

        /// <inheritdoc />
        public Task CreateAsync(
            ITenant tenant,
            Guid operationId,
            string? resourceLocation,
            long? expireAfter,
            string? clientData)
        {
            return this.SetStatusAsync(OperationStatus.NotStarted, tenant, operationId, resourceLocation: resourceLocation, clientData: clientData);
        }

        /// <inheritdoc />
        public Task SetRunningAsync(
            ITenant tenant,
            Guid operationId,
            int? percentComplete,
            string? contentId,
            long? expireAfter,
            string? clientData)
        {
            return this.SetStatusAsync(OperationStatus.Running, tenant, operationId, percentComplete, contentId: contentId, clientData: clientData);
        }

        /// <inheritdoc />
        public Task SetSucceededAsync(
            ITenant tenant,
            Guid operationId,
            string? resourceLocation,
            long? expireAfter,
            string? clientData)
        {
            return this.SetStatusAsync(OperationStatus.Succeeded, tenant, operationId, 100, resourceLocation: resourceLocation, clientData: clientData);
        }

        /// <inheritdoc />
        public Task SetFailedAsync(
            ITenant tenant,
            Guid operationId,
            long? expireAfter,
            string? clientData)
        {
            return this.SetStatusAsync(OperationStatus.Failed, tenant, operationId, clientData: clientData);
        }

        private async Task SetStatusAsync(
            OperationStatus status,
            ITenant tenant,
            Guid operationId,
            int? percentComplete = null,
            string? resourceLocation = null,
            string? contentId = null,
            string? clientData = null)
        {
            Operation? currentStatus = await this.operationRepository.GetAsync(tenant, operationId).ConfigureAwait(false);

            if (currentStatus == null)
            {
                currentStatus = new Operation(
                    operationId, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow, status, tenant.Id)
                {
                    ContentId = contentId,
                    PercentComplete = percentComplete,
                    ResourceLocation = resourceLocation,
                    ClientData = clientData,
                };
            }
            else
            {
                currentStatus.LastActionDateTime = DateTimeOffset.UtcNow;

                if (percentComplete.HasValue
                    && (!currentStatus.PercentComplete.HasValue || (currentStatus.PercentComplete < percentComplete)))
                {
                    currentStatus.PercentComplete = percentComplete;
                }

                if (string.IsNullOrEmpty(currentStatus.ResourceLocation) && !string.IsNullOrEmpty(resourceLocation))
                {
                    currentStatus.ResourceLocation = resourceLocation;
                }

                if (!string.IsNullOrEmpty(clientData))
                {
                    currentStatus.ClientData = clientData;
                }

                currentStatus.Status = status;
                currentStatus.ContentId = contentId;
            }

            await this.operationRepository.PersistAsync(tenant, currentStatus).ConfigureAwait(false);
        }
    }
}