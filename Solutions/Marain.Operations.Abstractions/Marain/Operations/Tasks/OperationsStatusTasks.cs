// <copyright file="OperationsStatusTasks.cs" company="Endjin Limited">
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
    public class OperationsStatusTasks : IOperationsStatusTasks
    {
        private readonly IOperationsRepository operationRepository;

        /// <summary>
        /// Creates a new <see cref="OperationsStatusTasks"/>.
        /// </summary>
        /// <param name="operationRepository">Storage.</param>
        public OperationsStatusTasks(IOperationsRepository operationRepository)
        {
            this.operationRepository = operationRepository;
        }

        /// <inheritdoc />
        public Task<Operation?> GetAsync(ITenant tenant, Guid operationId)
        {
            return this.operationRepository.GetAsync(tenant, operationId);
        }
    }
}