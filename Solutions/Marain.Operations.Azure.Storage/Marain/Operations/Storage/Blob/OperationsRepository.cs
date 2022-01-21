// <copyright file="OperationsRepository.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Operations.Storage.Blob
{
    using System;
    using System.Threading.Tasks;

    using Azure;
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
    using Azure.Storage.Blobs.Specialized;

    using Corvus.Extensions.Json;
    using Corvus.Storage.Azure.BlobStorage.Tenancy;
    using Corvus.Tenancy;

    using Newtonsoft.Json;

    using Operation = Marain.Operations.Domain.Operation;

    /// <summary>
    /// Azure blob storage implementation of Operations repository.
    /// </summary>
    public class OperationsRepository : IOperationsRepository
    {
        /// <summary>
        /// The container definition for the underlying blob container.
        /// </summary>
        public const string ContainerName = "operations";

        /// <summary>
        /// Property key with which legacy storage configuration settings are stored in the
        /// tenant properties.
        /// </summary>
        public const string OperationsV2ConfigKey = "StorageConfiguration__" + ContainerName;

        /// <summary>
        /// Property key with which modern storage configuration settings are stored in the
        /// tenant properties.
        /// </summary>
        public const string OperationsV3ConfigKey = "StorageConfigurationV3__" + ContainerName;

        private readonly IBlobContainerSourceWithTenantLegacyTransition containerSource;
        private readonly JsonSerializerSettings serializerSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationsRepository"/> class.
        /// </summary>
        /// <param name="containerFactory">The blob container factory to use to get the container in which operations should be stored.</param>
        /// <param name="serializerSettingsProvider">The serializer settings factory.</param>
        public OperationsRepository(
            IBlobContainerSourceWithTenantLegacyTransition containerFactory,
            IJsonSerializerSettingsProvider serializerSettingsProvider)
        {
            this.containerSource = containerFactory;
            this.serializerSettings = serializerSettingsProvider.Instance;
        }

        /// <inheritdoc />
        public async Task<Operation?> GetAsync(ITenant tenant, Guid operationId)
        {
            BlobContainerClient container = await this.containerSource.GetBlobContainerClientFromTenantAsync(
                tenant,
                OperationsV2ConfigKey,
                OperationsV3ConfigKey)
                .ConfigureAwait(false);

            BlockBlobClient blob = container.GetBlockBlobClient(GetBlobName(tenant, operationId));
            Response<BlobDownloadResult> response;
            try
            {
                response = await blob.DownloadContentAsync().ConfigureAwait(false);
            }
            catch (Azure.RequestFailedException rfx)
            when (rfx.Status == 404)
            {
                return null;
            }

            // Note: although BlobDownloadResult supports direct deserialization from JSON, using System.Text.Json
            // (meaning it can work directly with UTF-8 content, avoiding the conversion to UTF-16 we're doing
            // here) we currently depend on the JSON.NET serialization settings mechanism, so we have to use
            // this more inefficient route for now.
            string json = response.Value.Content.ToString();
            return JsonConvert.DeserializeObject<Operation>(json, this.serializerSettings);
        }

        /// <inheritdoc />
        public async Task PersistAsync(ITenant tenant, Operation operation)
        {
            BlobContainerClient container = await this.containerSource.GetBlobContainerClientFromTenantAsync(
                tenant,
                OperationsV2ConfigKey,
                OperationsV3ConfigKey)
                .ConfigureAwait(false);

            BlobClient blob = container.GetBlobClient(GetBlobName(tenant, operation.Id));

            string json = JsonConvert.SerializeObject(operation, this.serializerSettings);

            await blob.UploadAsync(
                BinaryData.FromString(json),
                overwrite: true)
                .ConfigureAwait(false);
        }

        private static string GetBlobName(ITenant tenant, Guid operationId) => $"{tenant.Id}/{operationId}";
    }
}