// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Marain.Operations.Client.OperationsControl
{
    using Models;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods for MarainOperationsControl.
    /// </summary>
    public static partial class MarainOperationsControlExtensions
    {
            /// <summary>
            /// Create a new operation, which will be in the "Not Started" state
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='tenantId'>
            /// Id of the Tenant.
            /// </param>
            /// <param name='operationId'>
            /// Id of the Operation.
            /// </param>
            /// <param name='resourceLocation'>
            /// Optional URI based location of the result of the Operation.
            /// </param>
            /// <param name='expireAfter'>
            /// Optional number of seconds for which to retain data about this operation
            /// after its last state change
            /// </param>
            /// <param name='body'>
            /// </param>
            public static CreateOperationHeaders CreateOperation(this IMarainOperationsControl operations, string tenantId, System.Guid operationId, string resourceLocation = default(string), long? expireAfter = default(long?), string body = default(string))
            {
                return operations.CreateOperationAsync(tenantId, operationId, resourceLocation, expireAfter, body).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Create a new operation, which will be in the "Not Started" state
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='tenantId'>
            /// Id of the Tenant.
            /// </param>
            /// <param name='operationId'>
            /// Id of the Operation.
            /// </param>
            /// <param name='resourceLocation'>
            /// Optional URI based location of the result of the Operation.
            /// </param>
            /// <param name='expireAfter'>
            /// Optional number of seconds for which to retain data about this operation
            /// after its last state change
            /// </param>
            /// <param name='body'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<CreateOperationHeaders> CreateOperationAsync(this IMarainOperationsControl operations, string tenantId, System.Guid operationId, string resourceLocation = default(string), long? expireAfter = default(long?), string body = default(string), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.CreateOperationWithHttpMessagesAsync(tenantId, operationId, resourceLocation, expireAfter, body, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Headers;
                }
            }

            /// <summary>
            /// Set an existing operation into the "Failed" state
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='tenantId'>
            /// Id of the Tenant.
            /// </param>
            /// <param name='operationId'>
            /// Id of the Operation.
            /// </param>
            /// <param name='expireAfter'>
            /// Optional number of seconds for which to retain data about this operation
            /// after its last state change
            /// </param>
            /// <param name='body'>
            /// </param>
            public static ProblemDetails SetOperationFailed(this IMarainOperationsControl operations, string tenantId, System.Guid operationId, long? expireAfter = default(long?), string body = default(string))
            {
                return operations.SetOperationFailedAsync(tenantId, operationId, expireAfter, body).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Set an existing operation into the "Failed" state
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='tenantId'>
            /// Id of the Tenant.
            /// </param>
            /// <param name='operationId'>
            /// Id of the Operation.
            /// </param>
            /// <param name='expireAfter'>
            /// Optional number of seconds for which to retain data about this operation
            /// after its last state change
            /// </param>
            /// <param name='body'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<ProblemDetails> SetOperationFailedAsync(this IMarainOperationsControl operations, string tenantId, System.Guid operationId, long? expireAfter = default(long?), string body = default(string), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.SetOperationFailedWithHttpMessagesAsync(tenantId, operationId, expireAfter, body, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Set an operation into the "Running" state
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='tenantId'>
            /// Id of the Tenant.
            /// </param>
            /// <param name='operationId'>
            /// Id of the Operation.
            /// </param>
            /// <param name='percentComplete'>
            /// Optional percentage completeness of the Operation.
            /// </param>
            /// <param name='contentId'>
            /// Optional Content Id of Localized Content status message.
            /// </param>
            /// <param name='expireAfter'>
            /// Optional number of seconds for which to retain data about this operation
            /// after its last state change
            /// </param>
            /// <param name='body'>
            /// </param>
            public static ProblemDetails SetOperationRunning(this IMarainOperationsControl operations, string tenantId, System.Guid operationId, int? percentComplete = default(int?), string contentId = default(string), long? expireAfter = default(long?), string body = default(string))
            {
                return operations.SetOperationRunningAsync(tenantId, operationId, percentComplete, contentId, expireAfter, body).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Set an operation into the "Running" state
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='tenantId'>
            /// Id of the Tenant.
            /// </param>
            /// <param name='operationId'>
            /// Id of the Operation.
            /// </param>
            /// <param name='percentComplete'>
            /// Optional percentage completeness of the Operation.
            /// </param>
            /// <param name='contentId'>
            /// Optional Content Id of Localized Content status message.
            /// </param>
            /// <param name='expireAfter'>
            /// Optional number of seconds for which to retain data about this operation
            /// after its last state change
            /// </param>
            /// <param name='body'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<ProblemDetails> SetOperationRunningAsync(this IMarainOperationsControl operations, string tenantId, System.Guid operationId, int? percentComplete = default(int?), string contentId = default(string), long? expireAfter = default(long?), string body = default(string), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.SetOperationRunningWithHttpMessagesAsync(tenantId, operationId, percentComplete, contentId, expireAfter, body, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Set an operation into the "Succeeded" state
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='tenantId'>
            /// Id of the Tenant.
            /// </param>
            /// <param name='operationId'>
            /// Id of Operation.
            /// </param>
            /// <param name='resourceLocation'>
            /// Optional URI based location of the result of the Operation.
            /// </param>
            /// <param name='expireAfter'>
            /// Optional number of seconds for which to retain data about this operation
            /// after its last state change
            /// </param>
            /// <param name='body'>
            /// </param>
            public static ProblemDetails SetOperationSucceeded(this IMarainOperationsControl operations, string tenantId, System.Guid operationId, string resourceLocation = default(string), long? expireAfter = default(long?), string body = default(string))
            {
                return operations.SetOperationSucceededAsync(tenantId, operationId, resourceLocation, expireAfter, body).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Set an operation into the "Succeeded" state
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='tenantId'>
            /// Id of the Tenant.
            /// </param>
            /// <param name='operationId'>
            /// Id of Operation.
            /// </param>
            /// <param name='resourceLocation'>
            /// Optional URI based location of the result of the Operation.
            /// </param>
            /// <param name='expireAfter'>
            /// Optional number of seconds for which to retain data about this operation
            /// after its last state change
            /// </param>
            /// <param name='body'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<ProblemDetails> SetOperationSucceededAsync(this IMarainOperationsControl operations, string tenantId, System.Guid operationId, string resourceLocation = default(string), long? expireAfter = default(long?), string body = default(string), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.SetOperationSucceededWithHttpMessagesAsync(tenantId, operationId, resourceLocation, expireAfter, body, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// View swagger definition for this API
            /// </summary>
            /// <remarks>
            /// View swagger definition for this API
            /// </remarks>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            public static object GetSwagger(this IMarainOperationsControl operations)
            {
                return operations.GetSwaggerAsync().GetAwaiter().GetResult();
            }

            /// <summary>
            /// View swagger definition for this API
            /// </summary>
            /// <remarks>
            /// View swagger definition for this API
            /// </remarks>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<object> GetSwaggerAsync(this IMarainOperationsControl operations, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.GetSwaggerWithHttpMessagesAsync(null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

    }
}