// <copyright file="OperationsControlHost.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Operations.ControlHost
{
    using System.Threading;
    using System.Threading.Tasks;

    using Menes;
    using Menes.Hosting.AzureFunctionsWorker;

    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Azure.Functions.Worker.Http;

    /// <summary>
    /// The host for the operations control services.
    /// </summary>
    public class OperationsControlHost
    {
        private readonly IOpenApiHost<HttpRequestData, IHttpResponseDataResult> host;

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationsControlHost"/> class.
        /// </summary>
        /// <param name="host">The OpenApi host.</param>
        public OperationsControlHost(IOpenApiHost<HttpRequestData, IHttpResponseDataResult> host)
        {
            this.host = host;
        }

        /// <summary>
        /// Azure Functions entry point.
        /// </summary>
        /// <param name="req">The <see cref="HttpRequestData"/>.</param>
        /// <param name="executionContext">The context for the function execution.</param>
        /// <returns>An action result which comes from executing the function.</returns>
        [Function("OperationsControlHost-OpenApiHostRoot")]
        public Task<HttpResponseData> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "put", Route = "{*path}")]
            HttpRequestData req,
            ExecutionContext executionContext)
        {
            return this.host.HandleRequestAsync(req, new { ExecutionContext = executionContext });
        }
    }
}