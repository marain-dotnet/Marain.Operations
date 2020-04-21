// <copyright file="TransientTenantManagerBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Testing
{
    using System.Threading.Tasks;
    using TechTalk.SpecFlow;

    /// <summary>
    /// SpecFlow bindings for cleaning up transient tenants.
    /// </summary>
    [Binding]
    public static class TransientTenantManagerBindings
    {
        /// <summary>
        /// Cleans up transient tenants post-feature.
        /// </summary>
        /// <param name="featureContext">The current feature context.</param>
        /// <returns>A task which completes when cleanup has finished.</returns>
        [AfterFeature]
        public static Task TearDownTenants(FeatureContext featureContext)
        {
            var tenantManager = TransientTenantManager.GetInstance(featureContext);
            return tenantManager.CleanupAsync();
        }
    }
}
