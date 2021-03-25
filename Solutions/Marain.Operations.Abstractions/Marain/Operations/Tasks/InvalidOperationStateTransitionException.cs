// <copyright file="InvalidOperationStateTransitionException.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Operations.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Corvus.Tenancy;
    using Marain.Operations.Domain;
    using Menes;

    /// <summary>
    /// Exception indicating that an attempt was made to move an Operation into a state that is not
    /// valid, given its current state.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is thrown by the various methods in <see cref="IOperationsControlTasks"/> when invalid
    /// transitions are attempted. For example, it is illegal to call
    /// <see cref="IOperationsControlTasks.CreateAsync(ITenant, Guid, string, long?, string)"/>
    /// if an operation with the specified id exists and is already in some state other than
    /// <see cref="OperationStatus.NotStarted"/>.
    /// </para>
    /// </remarks>
    [Serializable]
    public class InvalidOperationStateTransitionException : Exception
    {
        /// <summary>
        /// Create a <see cref="InvalidOperationStateTransitionException"/>.
        /// </summary>
        /// <param name="operationId">
        /// The unique identifier of the operation for which the state change was attempted.
        /// </param>
        /// <param name="currentState">The operation's current state.</param>
        /// <param name="attemptedTargetState">The state to which we tried to transition.</param>
        /// <param name="validTargetStates">
        /// The states to which it is possible to transition, given the current state.
        /// </param>
        /// <param name="validPriorStates">
        /// The states from which it would be possible to transition into the desired target state.
        /// </param>
        public InvalidOperationStateTransitionException(
            string operationId,
            OperationStatus currentState,
            OperationStatus attemptedTargetState,
            IEnumerable<OperationStatus> validTargetStates,
            IEnumerable<OperationStatus> validPriorStates)
            : base($"Operation [{operationId}] cannot transition from state [{currentState}] to state [{attemptedTargetState}]")
        {
            this.AddProblemDetailsTitle("Invalid Operation State Transition")
                .AddProblemDetailsExplanation(this.Message)
                .AddProblemDetailsExtension(nameof(this.CurrentState), this.CurrentState)
                .AddProblemDetailsExtension(nameof(this.AttemptedTargetState), this.AttemptedTargetState)
                .AddProblemDetailsExtension("ValidTargetStates", validTargetStates.ToList())
                .AddProblemDetailsExtension("ValidPriorStates", validPriorStates.ToList());
        }

        /// <summary>
        /// Standard constructor for any derived exceptions.
        /// </summary>
        protected InvalidOperationStateTransitionException()
        {
        }

        /// <summary>
        /// Standard constructor for any derived exceptions.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        protected InvalidOperationStateTransitionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Standard constructor for any derived exceptions.
        /// </summary>
        /// <param name="message">The exception message.</param>
        protected InvalidOperationStateTransitionException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="info">Serialized information container.</param>
        /// <param name="context">Additional serialization details.</param>
        protected InvalidOperationStateTransitionException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets the unique identifier of the operation for which the state change was attempted.
        /// </summary>
        public Guid OperationId { get; }

        /// <summary>
        /// Gets the state the operation was in when the transition was attempted.
        /// </summary>
        public OperationStatus CurrentState { get; }

        /// <summary>
        /// Gets the state to which an transition was attempted.
        /// </summary>
        public OperationStatus AttemptedTargetState { get; }
    }
}
