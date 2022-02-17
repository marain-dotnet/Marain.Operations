// <copyright file="OperationsControlOpenApiService.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Operations.OpenApi
{
    using System;
    using System.Threading.Tasks;
    using Corvus.Tenancy;
    using Marain.Operations.Domain;
    using Marain.Operations.Tasks;
    using Marain.Services.Tenancy;
    using Menes;

    /// <summary>
    /// OpenApi service enabling services offering long-running operations to define and update
    /// the representations of those operations presented by <see cref="OperationsStatusOpenApiService"/>.
    /// </summary>
    [EmbeddedOpenApiDefinition("Marain.Operations.OpenApi.OperationsControl.yaml")]
    public class OperationsControlOpenApiService : IOpenApiService
    {
        private readonly IMarainServicesTenancy tenancyHelper;
        private readonly IOperationsControlTasks tasks;
        private readonly IOpenApiExternalServices externalServiceResolver;

        /// <summary>
        /// Creates a <see cref="OperationsControlOpenApiService"/>.
        /// </summary>
        /// <param name="tenancyHelper">The tenancy helper.</param>
        /// <param name="tasks">The underlying tasks that implement the service functionality.</param>
        /// <param name="uriTemplateProvider">Resolves URLs to other services.</param>
        public OperationsControlOpenApiService(
            IMarainServicesTenancy tenancyHelper,
            IOperationsControlTasks tasks,
            IOpenApiExternalServices uriTemplateProvider)
        {
            this.tenancyHelper = tenancyHelper;
            this.tasks = tasks;
            this.externalServiceResolver = uriTemplateProvider;
        }

        /// <summary>
        /// Creates a new operation, which will be in the <see cref="OperationStatus.NotStarted"/> state.
        /// </summary>
        /// <param name="tenantId">The tenant id.</param>
        /// <param name="operationId">
        /// Client-supplied unique identifier for operation.
        /// </param>
        /// <param name="resourceLocation">
        /// Optional value to set as the operation's resource location.
        /// </param>
        /// <param name="expireAfter">
        /// Optional time to live. If set, the operation can be forgotten about after the number of
        /// seconds specified. If not specified, a configurable service-wide default will be used.
        /// </param>
        /// <param name="body">
        /// Arbitrary data that will be set as the operation's ClientData property.
        /// </param>
        /// <returns>A description of the HTTP response to produce.</returns>
        [OperationId(nameof(CreateOperation))]
        public async Task<OpenApiResult> CreateOperation(
            string tenantId,
            Guid operationId,
            string? resourceLocation = null,
            long? expireAfter = null,
            string? body = null)
        {
            ITenant tenant = await this.tenancyHelper.GetRequestingTenantAsync(tenantId).ConfigureAwait(false);

            await this.tasks.CreateAsync(tenant, operationId, resourceLocation, expireAfter, body).ConfigureAwait(false);

            // We return a 201 Created because by the time we return we have created the operation.
            // Although it might seem inconsistent with Microsoft guidance for returning 202 Accepted
            // for a long-running operation, remember that this particular endpoint is typically only
            // used internally: it will have been invoked by some other service which may then decide
            // to return a 202 Accepted to its caller. The point is that the resource *this* service has
            // been asked to create does now exist, regardless of whether the resource being created by
            // the long-running service exists.
            // Note that this is an idempotent operation: we tolerate creation of a task that has
            // already been created as long as it is still in the <see cref="OperationStatus.NotStarted"/>
            // state. This is to enable successful recovery if an earlier operation succeeded by the
            // client never saw the result. For this reason, we return a 201 Created even if the
            // resource already existed.
            return this.CreatedResultWithOperationLocationHeader(tenant.Id, operationId);
        }

        /// <summary>
        /// Sets an operation into the <see cref="OperationStatus.Failed"/> state.
        /// </summary>
        /// <param name="tenantId">The tenant id.</param>
        /// <param name="operationId">
        /// Client-supplied unique identifier for operation.
        /// </param>
        /// <param name="expireAfter">
        /// Optional time to live. If set, the operation can be forgotten about after the number of
        /// seconds specified. If not specified, the expiration time previously associated with this
        /// operation will be used. In either case, the timer is reset - if the expiration time is
        /// 300 seconds (whether set explicitly in this call, or determined by an earlier one) the
        /// operation will expire no sooner than 300 seconds after this call, no matter how long it
        /// has already been running for.
        /// </param>
        /// <param name="body">
        /// Arbitrary data that will be set as the operation's ClientData property.
        /// </param>
        /// <returns>A description of the HTTP response to produce.</returns>
        [OperationId(nameof(SetOperationFailed))]
        public async Task<OpenApiResult> SetOperationFailed(
            string tenantId,
            Guid operationId,
            long? expireAfter = null,
            string? body = null)
        {
            ITenant tenant = await this.tenancyHelper.GetRequestingTenantAsync(tenantId).ConfigureAwait(false);

            await this.tasks.SetFailedAsync(tenant, operationId, expireAfter, clientData: body).ConfigureAwait(false);

            // For this and all the remaining methods, the reason we return a 201 Created is
            // subtly different from the reason we do it in CreateOperation. With CreateOperation
            // we are obviously creating something new. With state changes, it seems like we are
            // not. However, we've chosen to model state changes with PUT operations to various
            // endpoints, so from an HTTP point of view, if that PUT succeeds, we must have
            // created something. (Logically speaking, the thing we've created is a state
            // transition. We don't actually provide any way of retrieving a representation of
            // that transition, but in theory it exists.) So a 201 Created is a suitable response.
            return this.CreatedResultWithOperationLocationHeader(tenant.Id, operationId);
        }

        /// <summary>
        /// Sets an operation into the <see cref="OperationStatus.Running"/> state.
        /// </summary>
        /// <param name="tenantId">The tenant id.</param>
        /// <param name="operationId">
        /// Client-supplied unique identifier for operation.
        /// </param>
        /// <param name="percentComplete">
        /// The proportion of the operation's work that is complete so far, expressed as a number
        /// from 0 to 100.</param>
        /// <param name="contentId">
        /// Optional identifier for a message describing the current state of the operation.
        /// </param>
        /// <param name="expireAfter">
        /// Optional time to live. If set, the operation can be forgotten about after the number of
        /// seconds specified. If not specified, the expiration time previously associated with this
        /// operation will be used. In either case, the timer is reset - if the expiration time is
        /// 300 seconds (whether set explicitly in this call, or determined by an earlier one) the
        /// operation will expire no sooner than 300 seconds after this call, no matter how long it
        /// has already been running for.
        /// </param>
        /// <param name="body">
        /// Arbitrary data that will be set as the operation's ClientData property.
        /// </param>
        /// <returns>A description of the HTTP response to produce.</returns>
        [OperationId(nameof(SetOperationRunning))]
        public async Task<OpenApiResult> SetOperationRunning(
            string tenantId,
            Guid operationId,
            int? percentComplete = null,
            string? contentId = null,
            long? expireAfter = null,
            string? body = null)
        {
            ITenant tenant = await this.tenancyHelper.GetRequestingTenantAsync(tenantId).ConfigureAwait(false);

            await this.tasks.SetRunningAsync(tenant, operationId, percentComplete, contentId, expireAfter, clientData: body).ConfigureAwait(false);

            return this.CreatedResultWithOperationLocationHeader(tenant.Id, operationId);
        }

        /// <summary>
        /// Sets an operation into the <see cref="OperationStatus.Succeeded"/> state.
        /// </summary>
        /// <param name="tenantId">The tenant id.</param>
        /// <param name="operationId">
        /// Client-supplied unique identifier for operation.
        /// </param>
        /// <param name="resourceLocation">
        /// Optional URI of a resource representing the outcome of the operation.
        /// </param>
        /// <param name="expireAfter">
        /// Optional time to live. If set, the operation can be forgotten about after the number of
        /// seconds specified. If not specified, the expiration time previously associated with this
        /// operation will be used. In either case, the timer is reset - if the expiration time is
        /// 300 seconds (whether set explicitly in this call, or determined by an earlier one) the
        /// operation will expire no sooner than 300 seconds after this call, no matter how long it
        /// has already been running for.
        /// </param>
        /// <param name="body">
        /// Arbitrary data that will be set as the operation's ClientData property.
        /// </param>
        /// <returns>A description of the HTTP response to produce.</returns>
        [OperationId(nameof(SetOperationSucceeded))]
        public async Task<OpenApiResult> SetOperationSucceeded(
            string tenantId,
            Guid operationId,
            string? resourceLocation = null,
            long? expireAfter = null,
            string? body = null)
        {
            ITenant tenant = await this.tenancyHelper.GetRequestingTenantAsync(tenantId).ConfigureAwait(false);

            await this.tasks.SetSucceededAsync(tenant, operationId, resourceLocation, expireAfter, clientData: body).ConfigureAwait(false);

            return this.CreatedResultWithOperationLocationHeader(tenant.Id, operationId);
        }

        private OpenApiResult CreatedResultWithOperationLocationHeader(string tenantId, Guid operationId)
        {
            Uri operationUrl = this.externalServiceResolver.ResolveUrl<OperationsStatusOpenApiService>(
                nameof(OperationsStatusOpenApiService.GetOperationById),
                ("tenantId", tenantId),
                ("operationId", operationId));
            return this.CreatedResult(operationUrl.ToString());
        }
    }
}