// <copyright file="OperationStatus.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Operations.Domain;

/// <summary>
/// Represents the status of a long-running operation.
/// </summary>
public enum OperationStatus
{
    /// <summary>
    /// The operation has not yet started work.
    /// </summary>
    NotStarted,

    /// <summary>
    /// The operation is in progress.
    /// </summary>
    Running,

    /// <summary>
    /// The operation has completed successfully.
    /// </summary>
    Succeeded,

    /// <summary>
    /// The operation failed.
    /// </summary>
    Failed,
}