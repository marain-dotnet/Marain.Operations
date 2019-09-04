// <copyright file="IOperationsStatusTasks.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Operations.Tasks
{
    using System;
    using System.Threading.Tasks;
    using Corvus.Tenancy;
    using Marain.Operations.Domain;

    /// <summary>
    /// The tasks underpinning the OperationsStatus service.
    /// </summary>
    public interface IOperationsStatusTasks
    {
        /// <summary>
        /// Get an operation.
        /// </summary>
        /// <param name="tenant">The owning tenant.</param>
        /// <param name="operationId">The operation id.</param>
        /// <returns>A task that completes when the data has been stored.</returns>
        Task<Operation> GetAsync(ITenant tenant, Guid operationId);
    }
}