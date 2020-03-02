// <copyright file="OperationsControlHost.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Operations.ControlHost
{
    using System.Threading.Tasks;
    using Menes;
    using Menes.Hosting.AspNetCore;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;

    /// <summary>
    /// The host for the operations control services.
    /// </summary>
    public class OperationsControlHost
    {
        private readonly IOpenApiHost<HttpRequest, IActionResult> host;

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationsControlHost"/> class.
        /// </summary>
        /// <param name="host">The OpenApi host.</param>
        public OperationsControlHost(IOpenApiHost<HttpRequest, IActionResult> host)
        {
            this.host = host;
        }

        /// <summary>
        /// Azure Functions entry point.
        /// </summary>
        /// <param name="req">The <see cref="HttpRequest"/>.</param>
        /// <param name="executionContext">The context for the function execution.</param>
        /// <returns>An action result which comes from executing the function.</returns>
        [FunctionName("OperationsControlHost-OpenApiHostRoot")]
        public Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", "put", Route = "{*path}")]HttpRequest req, ExecutionContext executionContext)
        {
            return this.host.HandleRequestAsync(req, new { ExecutionContext = executionContext });
        }
    }
}
