// <copyright file="Operation.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Operations.Domain
{
    using System;

    /// <summary>
    /// Information about a long-running operation.
    /// </summary>
    public class Operation
    {
        /// <summary>
        /// Creates a <see cref="Operation"/>.
        /// </summary>
        /// <param name="id">The <see cref="Id"/>.</param>
        /// <param name="createdDateTime">The <see cref="CreatedDateTime"/>.</param>
        /// <param name="lastActionDateTime">The <see cref="LastActionDateTime"/>.</param>
        /// <param name="status">The <see cref="Status"/>.</param>
        /// <param name="tenantId">The <see cref="TenantId"/>.</param>
        public Operation(
            Guid id,
            DateTimeOffset createdDateTime,
            DateTimeOffset lastActionDateTime,
            OperationStatus status,
            string tenantId)
        {
            this.Id = id;
            this.LastActionDateTime = lastActionDateTime;
            this.CreatedDateTime = createdDateTime;
            this.Status = status;
            this.TenantId = tenantId;
        }

        /// <summary>
        /// Gets or sets the time at which the operation was created.
        /// </summary>
        public DateTimeOffset CreatedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of this operation.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the time at which this operation's state last changed.
        /// </summary>
        public DateTimeOffset LastActionDateTime { get; set; }

        /// <summary>
        /// Gets or sets the identifier of some content representing information about the operation's state.
        /// </summary>
        /// <remarks>
        /// This is typically a key into some localizable string store. We expose the id here to
        /// avoid making the Operations API locale-specific.
        /// </remarks>
        public string? ContentId { get; set; }

        /// <summary>
        /// Gets or sets a value from 0 to 100 representing what proportion of the operation's work
        /// is complete.
        /// </summary>
        public int? PercentComplete { get; set; }

        /// <summary>
        /// Gets or sets the URL of a resource representing the outcome of the operation.
        /// </summary>
        public string? ResourceLocation { get; set; }

        /// <summary>
        /// Gets or sets the current status of this operation.
        /// </summary>
        public OperationStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the unique ID of the tenant in which this operation belongs.
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// Gets or sets an arbitrary string of data specific to the owner of the
        /// operation.
        /// </summary>
        /// <remarks>
        /// The operations API makes no assumptions about the content or structure of
        /// this data, other than it will be kept to a small size. This size limit
        /// will be enforced by the API.
        /// </remarks>
        public string? ClientData { get; set; }
    }
}