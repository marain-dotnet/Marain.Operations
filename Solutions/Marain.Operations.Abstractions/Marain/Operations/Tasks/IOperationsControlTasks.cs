// <copyright file="IOperationsControlTasks.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Operations.Tasks
{
    using System;
    using System.Threading.Tasks;
    using Corvus.Tenancy;
    using Marain.Operations.Domain;

    /// <summary>
    /// The tasks underpinning the OperationsControl service.
    /// </summary>
    /// <remarks>
    /// <para>
    /// All operations have a minimum time they will remain available for before expiring, at
    /// which point any storage associated with them may be deleted. Each change in state resets
    /// this timer. So an operation may be configured with a 120 second expiration duration, but
    /// if calls to <see cref="SetRunningAsync(ITenant, Guid, int?, string, long?, string)"/>
    /// occur every 10 seconds, the operation could run indefinitely without expiring, because
    /// each call resets the expiration timer. If the expiration time is not set explicitly on
    /// any particular call, the value previously specified will be used (or, for
    /// <see cref="CreateAsync(ITenant, Guid, string, long?, string)"/>, a configurable
    /// default is used), but the expiration timer will be reset on these calls too. (I.e., you
    /// don't need to specify the expiration duration on every call - the timer is reset in
    /// either case.)
    /// </para>
    /// </remarks>
    public interface IOperationsControlTasks
    {
        /// <summary>
        /// Create an new operation, leaving it in the Not Started state.
        /// </summary>
        /// <param name="tenant">The owning tenant.</param>
        /// <param name="operationId">The operation id.</param>
        /// <param name="resourceLocation">The optional resource location.</param>
        /// <param name="expireAfter">Optional time to live.</param>
        /// <param name="clientData">Optional data to be stored in the operation's ClientData property.</param>
        /// <returns>A task that completes when the data has been stored.</returns>
        Task CreateAsync(
            ITenant tenant,
            Guid operationId,
            string resourceLocation = "",
            long? expireAfter = null,
            string clientData = null);

        /// <summary>
        /// Put an operation into the Failed state.
        /// </summary>
        /// <param name="tenant">The owning tenant.</param>
        /// <param name="operationId">The operation id.</param>
        /// <param name="expireAfter">Optional time to live.</param>
        /// <param name="clientData">
        /// Optional data to be stored in the operation's <see cref="Operation.ClientData" />
        /// property. If no value is supplied, any previously supplied data will be retained.
        /// To clear a previously supplied value, pass <see cref="string.Empty"/> to this
        /// parameter.
        /// </param>
        /// <returns>A task that completes when the data has been stored.</returns>
        Task SetFailedAsync(
            ITenant tenant,
            Guid operationId,
            long? expireAfter = null,
            string clientData = null);

        /// <summary>
        /// Put an operation into the Running state, or update its status.
        /// </summary>
        /// <param name="tenant">The owning tenant.</param>
        /// <param name="operationId">The operation id.</param>
        /// <param name="percentComplete">
        /// The proportion of work completed, as a number from 0 to 100. If no value is supplied,
        /// or a value lower than one previously set is supplied, then the operation's
        /// <see cref="Operation.PercentComplete"/> property will not be updated.
        /// </param>
        /// <param name="contentId">Optional id of content describing the current state.</param>
        /// <param name="expireAfter">Optional time to live.</param>
        /// <param name="clientData">
        /// Optional data to be stored in the operation's <see cref="Operation.ClientData" />
        /// property. If no value is supplied, any previously supplied data will be retained.
        /// To clear a previously supplied value, pass <see cref="string.Empty"/> to this
        /// parameter.
        /// </param>
        /// <returns>A task that completes when the data has been stored.</returns>
        Task SetRunningAsync(
            ITenant tenant,
            Guid operationId,
            int? percentComplete = null,
            string contentId = "",
            long? expireAfter = null,
            string clientData = null);

        /// <summary>
        /// Put an operation into the Succeeded state.
        /// </summary>
        /// <param name="tenant">The owning tenant.</param>
        /// <param name="operationId">The operation id.</param>
        /// <param name="resourceLocation">
        /// The optional resource location. If no value is supplied, but a value was supplied
        /// in the call to <see cref="CreateAsync(ITenant, Guid, string, long?, string)"/>, the
        /// previously supplied value will be retained. Pass <see cref="string.Empty"/> to remove
        /// the a previously supplied value.
        /// </param>
        /// <param name="expireAfter">Optional time to live.</param>
        /// <param name="clientData">
        /// Optional data to be stored in the operation's <see cref="Operation.ClientData" />
        /// property. If no value is supplied, any previously supplied data will be retained.
        /// To clear a previously supplied value, pass <see cref="string.Empty"/> to this
        /// parameter.
        /// </param>
        /// <returns>A task that completes when the data has been stored.</returns>
        Task SetSucceededAsync(
            ITenant tenant,
            Guid operationId,
            string resourceLocation = "",
            long? expireAfter = null,
            string clientData = null);
    }
}