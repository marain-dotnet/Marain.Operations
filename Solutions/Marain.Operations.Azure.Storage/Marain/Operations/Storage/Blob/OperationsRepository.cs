// <copyright file="OperationsRepository.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Operations.Storage.Blob
{
    using System;
    using System.Threading.Tasks;
    using Corvus.Azure.Storage.Tenancy;
    using Corvus.Extensions.Json;
    using Corvus.Tenancy;
    using Marain.Operations.Domain;
    using Microsoft.Azure.Storage.Blob;
    using Newtonsoft.Json;

    /// <summary>
    /// Azure blob storage implementation of Operations repository.
    /// </summary>
    public class OperationsRepository : IOperationsRepository
    {
        private readonly BlobStorageContainerDefinition containerDefinition;
        private readonly ITenantCloudBlobContainerFactory containerFactory;
        private readonly JsonSerializerSettings serializerSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationsRepository"/> class.
        /// </summary>
        /// <param name="containerFactory">The blob container factory to use to get the container in which operations should be stored.</param>
        /// <param name="serializerSettingsProvider">The serializer settings factory.</param>
        public OperationsRepository(ITenantCloudBlobContainerFactory containerFactory, IJsonSerializerSettingsProvider serializerSettingsProvider)
        {
            this.containerDefinition = new BlobStorageContainerDefinition("operations");
            this.containerFactory = containerFactory;
            this.serializerSettings = serializerSettingsProvider.Instance;
        }

        /// <inheritdoc />
        public async Task<Operation> GetAsync(ITenant tenant, Guid operationId)
        {
            CloudBlobContainer container = await this.containerFactory.GetBlobContainerForTenantAsync(tenant, this.containerDefinition).ConfigureAwait(false);

            CloudBlockBlob blob = container.GetBlockBlobReference(GetBlobName(tenant, operationId));

            bool exists = await blob.ExistsAsync().ConfigureAwait(false);

            if (!exists)
            {
                return null;
            }

            string json = await blob.DownloadTextAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<Operation>(json, this.serializerSettings);
        }

        /// <inheritdoc />
        public async Task PersistAsync(ITenant tenant, Operation operation)
        {
            CloudBlobContainer container = await this.containerFactory.GetBlobContainerForTenantAsync(tenant, this.containerDefinition).ConfigureAwait(false);

            CloudBlockBlob blob = container.GetBlockBlobReference(GetBlobName(tenant, operation.Id));

            string json = JsonConvert.SerializeObject(operation, this.serializerSettings);

            await blob.UploadTextAsync(json).ConfigureAwait(false);
        }

        private static string GetBlobName(ITenant tenant, Guid operationId) => $"{tenant.Id}/{operationId}";
    }
}