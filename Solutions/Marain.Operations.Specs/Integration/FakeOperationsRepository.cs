// <copyright file="FakeOperationsRepository.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Operations.Specs.Integration;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Corvus.Tenancy;
using Marain.Operations.Domain;
using Marain.Operations.Storage;

/// <summary>
/// In-memory repository for test purposes.
/// </summary>
public class FakeOperationsRepository : IOperationsRepository
{
    private readonly Dictionary<(string TenantId, Guid OperationId), Operation> operations = new();

    /// <inheritdoc />
    public Task<Operation?> GetAsync(ITenant tenant, Guid operationId)
    {
        this.operations.TryGetValue((tenant.Id, operationId), out Operation? result);
        return Task.FromResult(result);
    }

    /// <inheritdoc />
    public Task PersistAsync(ITenant tenant, Operation operation)
    {
        if (tenant.Id != operation.TenantId)
        {
            throw new ArgumentException($"Tenant id in 'tenant' argument ('{tenant.Id}') does not match one in 'operation' ('{operation.TenantId}')");
        }

        this.operations[(operation.TenantId, operation.Id)] = operation;
        return Task.CompletedTask;
    }

    /// <summary>
    /// Resets the store.
    /// </summary>
    public void Reset()
    {
        this.operations.Clear();
    }
}