// <copyright file="IOperationsRepository.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Operations.Storage;

using System;
using System.Threading.Tasks;
using Corvus.Tenancy;
using Marain.Operations.Domain;

/// <summary>
/// Defines the persistence operations required by the Operations service.
/// </summary>
public interface IOperationsRepository
{
    /// <summary>
    /// Gets a specific operation.
    /// </summary>
    /// <param name="tenant">The operation's owning tenant.</param>
    /// <param name="operationId">The operation's unique id.</param>
    /// <returns>
    /// A task that produces either the specified operation, or null if no operation with the
    /// specified id exists in the tenant.
    /// </returns>
    Task<Operation?> GetAsync(ITenant tenant, Guid operationId);

    /// <summary>
    /// Saves changes to an operation.
    /// </summary>
    /// <param name="tenant">The operation's owning tenant.</param>
    /// <param name="operation">The updated operation to save.</param>
    /// <returns>A task that completes when the data has been saved.</returns>
    /// <remarks>
    /// Implementations are not required to check for conflicts when updating
    /// existing entries; concurrent updates to the same <see cref="Operation"/>
    /// are not supported.
    /// </remarks>
    Task PersistAsync(ITenant tenant, Operation operation);
}