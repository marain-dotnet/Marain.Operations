// <copyright file="OperationsRepository.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Operations.Storage.Blob
{
    using System;
    using System.Text.Json;
    using System.Threading.Tasks;

    using Azure;
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
    using Azure.Storage.Blobs.Specialized;

    using CommunityToolkit.HighPerformance.Buffers;

    using Corvus.Storage.Azure.BlobStorage.Tenancy;
    using Corvus.Tenancy;

    using Microsoft.Extensions.ObjectPool;

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
        public const string OperationsV3ConfigKey = "Marain:Operations:BlobContainerConfiguration:Operations";

        private static readonly ObjectPool<ArrayPoolBufferWriter<byte>> ArrayPoolWriterPool =
            new DefaultObjectPoolProvider().Create<ArrayPoolBufferWriter<byte>>();

        private readonly IBlobContainerSourceWithTenantLegacyTransition containerSource;
        private readonly JsonSerializerOptions serializerOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationsRepository"/> class.
        /// </summary>
        /// <param name="containerFactory">The blob container factory to use to get the container in which operations should be stored.</param>
        /// <param name="serializerOptions">The serializer settings.</param>
        internal OperationsRepository(
            IBlobContainerSourceWithTenantLegacyTransition containerFactory,
            JsonSerializerOptions serializerOptions)
        {
            this.containerSource = containerFactory;
            this.serializerOptions = serializerOptions;
        }

        /// <inheritdoc />
        public async Task<Operation?> GetAsync(ITenant tenant, Guid operationId)
        {
            BlobContainerClient container = await this.containerSource.GetBlobContainerClientFromTenantAsync(
                tenant,
                OperationsV2ConfigKey,
                OperationsV3ConfigKey,
                ContainerName)
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

            return response.Value.Content.ToObjectFromJson<Operation>(this.serializerOptions);
        }

        /// <inheritdoc />
        public async Task PersistAsync(ITenant tenant, Operation operation)
        {
            BlobContainerClient container = await this.containerSource.GetBlobContainerClientFromTenantAsync(
                tenant,
                OperationsV2ConfigKey,
                OperationsV3ConfigKey,
                ContainerName)
                .ConfigureAwait(false);

            BlobClient blob = container.GetBlobClient(GetBlobName(tenant, operation.Id));

            // Note: we could use this:
            //  BinaryData.FromObjectAsJson(operation, this.serializerOptions);
            // However, that calls JsonSerializer.SerializeToUtf8Bytes, which returns a byte[]
            // which is always allocated as a new heap entry. This following code uses an array
            // pool, meaning that we can reuse the buffer across multiple calls, reducing the
            // number of heap allocations when this is called multiple times.
            ArrayPoolBufferWriter<byte>? json = null;
            try
            {
                json = ArrayPoolWriterPool.Get();
                using (Utf8JsonWriter jw = new(json))
                {
                    JsonSerializer.Serialize(jw, operation, this.serializerOptions);
                }

                await blob.UploadAsync(
                    BinaryData.FromBytes(json.WrittenMemory),
                    overwrite: true)
                    .ConfigureAwait(false);
            }
            finally
            {
                if (json is not null)
                {
                    json.Clear();
                    ArrayPoolWriterPool.Return(json);
                }
            }
        }

        private static string GetBlobName(ITenant tenant, Guid operationId) => $"{tenant.Id}/{operationId}";
    }
}