// <copyright file="TransientTenantManager.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.TenantManagement.Testing
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Corvus.Extensions.Json;
    using Corvus.SpecFlow.Extensions;
    using Corvus.Tenancy;
    using Marain.TenantManagement;
    using Marain.TenantManagement.EnrollmentConfiguration;
    using Marain.TenantManagement.ServiceManifests;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Helper methods for creating transient client and service tenants. Tenants and enrollments
    /// are tracked and then can be cleaned up via a call to <see cref="CleanupAsync"/>.
    /// </summary>
    public class TransientTenantManager
    {
        private readonly ITenantManagementService tenantManagementService;
        private readonly ITenantProvider tenantProvider;
        private readonly IJsonSerializerSettingsProvider jsonSerializerSettingsProvider;
        private List<ITenant> tenants = new List<ITenant>();
        private List<(string EnrolledTenantId, string ServiceTenantId)> enrollments = new List<(string EnrolledTenant, string ServiceTenant)>();
        private ITenant? primaryTransientClient;

        private TransientTenantManager(
            ITenantManagementService tenantManagementService,
            ITenantProvider tenantProvider,
            IJsonSerializerSettingsProvider jsonSerializerSettingsProvider)
        {
            this.tenantManagementService = tenantManagementService;
            this.tenantProvider = tenantProvider;
            this.jsonSerializerSettingsProvider = jsonSerializerSettingsProvider;
        }

        /// <summary>
        /// Gets or sets the tenant that is being used as the primary client for test purposes.
        /// </summary>
        /// <remarks>
        /// It will be normal as part of specs to have a client that's being used to access services. This property allows you
        /// to store that tenant here as a shortcut. By default it will be set to the first client created.
        /// </remarks>
        public ITenant PrimaryTransientClient
        {
            get { return this.primaryTransientClient ?? throw new InvalidOperationException("The primary transient client tenant has not been set."); }
            set { this.primaryTransientClient = value ?? throw new ArgumentException(nameof(this.PrimaryTransientClient)); }
        }

        /// <summary>
        /// Retrieves the instance for the current feature.
        /// </summary>
        /// <param name="featureContext">The current feature context.</param>
        /// <returns>The instance for the current feature.</returns>
        /// <remarks>
        /// When this is called for the first time, the service provider will be retrieved from the
        /// <see cref="FeatureContext"/> and used to obtain instances of:
        /// <list type="bullet">
        ///     <item><see cref="ITenantManagementService"/></item>
        ///     <item><see cref="ITenantProvider"/></item>
        ///     <item><see cref="IJsonSerializerSettingsProvider"/></item>
        /// </list>
        /// The <see cref="ITenantManagementService"/> must already be initialised for use with
        /// Marain.
        /// </remarks>
        public static TransientTenantManager GetInstance(FeatureContext featureContext)
        {
            if (!featureContext.TryGetValue(out TransientTenantManager helper))
            {
                IServiceProvider serviceProvider = ContainerBindings.GetServiceProvider(featureContext);

                helper = new TransientTenantManager(
                    serviceProvider.GetRequiredService<ITenantManagementService>(),
                    serviceProvider.GetRequiredService<ITenantProvider>(),
                    serviceProvider.GetRequiredService<IJsonSerializerSettingsProvider>());

                featureContext.Set(helper);
            }

            return helper;
        }

        /// <summary>
        /// Ensures that the underlying <see cref="ITenantManagementService"/> is initialised for use with
        /// Marain.
        /// </summary>
        /// <returns>A task which completes when the operation is finished.</returns>
        public Task EnsureInitialised()
        {
            return this.tenantManagementService.InitialiseTenancyProviderAsync();
        }

        /// <summary>
        /// Creates a new transient service tenant using a service manifest provided as an embedded resource.
        /// </summary>
        /// <param name="assembly">The assembly containing the manifest JSON resource.</param>
        /// <param name="name">The name of the embedded resource.</param>
        /// <returns>The new transient tenant.</returns>
        public Task<ITenant> CreateTransientServiceTenantFromEmbeddedResourceAsync(
            Assembly assembly,
            string name)
        {
            using Stream stream = assembly.GetManifestResourceStream(name)
                ?? throw new ArgumentException($"Could not find an embedded resource named '{name}'");

            return this.CreateTransientServiceTenantFromManifestStreamAsync(stream);
        }

        /// <summary>
        /// Creates a new transient service tenant by changing the well known Guid the manifest contained in the provided
        /// strean.
        /// </summary>
        /// <param name="serviceManifestStream">The stream containing the manifest JSON data.</param>
        /// <returns>The new transient tenant.</returns>
        public Task<ITenant> CreateTransientServiceTenantFromManifestStreamAsync(
            Stream serviceManifestStream)
        {
            using var reader = new StreamReader(serviceManifestStream);

            string manifestJson = reader.ReadToEnd();

            return this.CreateTransientServiceTenantAsync(manifestJson);
        }

        /// <summary>
        /// Creates a new transient service tenant by changing the well known Guid in the provided
        /// manifest.
        /// </summary>
        /// <param name="serviceManifestJson">A string containing the manifest JSON data.</param>
        /// <returns>The new transient tenant.</returns>
        public Task<ITenant> CreateTransientServiceTenantAsync(string serviceManifestJson)
        {
            ServiceManifest manifest = JsonConvert.DeserializeObject<ServiceManifest>(
                serviceManifestJson,
                this.jsonSerializerSettingsProvider.Instance);

            return this.CreateTransientServiceTenantAsync(manifest);
        }

        /// <summary>
        /// Creates a new transient service tenant by changing the well known Guid in the provided
        /// manifest.
        /// </summary>
        /// <param name="manifest">The manifest for the service.</param>
        /// <returns>The new transient tenant.</returns>
        public async Task<ITenant> CreateTransientServiceTenantAsync(ServiceManifest manifest)
        {
            manifest.WellKnownTenantGuid = Guid.NewGuid();
            manifest.ServiceName = $"{manifest.ServiceName} - {manifest.WellKnownTenantGuid}";

            ITenant serviceTenant =
                await this.tenantManagementService.CreateServiceTenantAsync(manifest).ConfigureAwait(false);

            this.tenants.Add(serviceTenant);

            return serviceTenant;
        }

        /// <summary>
        /// Adds a service enrollment. Using this method means that the enrollment will be reversed as part of the
        /// test cleanup.
        /// </summary>
        /// <param name="enrollingTenantId">The Id of the tenant to enroll.</param>
        /// <param name="serviceTenantId">The Id of the service to enroll in.</param>
        /// <param name="configurationItems">Configuration for the enrollment.</param>
        /// <returns>A task which completes when the enrollment has finished.</returns>
        public async Task AddEnrollmentAsync(
            string enrollingTenantId,
            string serviceTenantId,
            EnrollmentConfigurationItem[] configurationItems)
        {
            await this.tenantManagementService.EnrollInServiceAsync(
                enrollingTenantId,
                serviceTenantId,
                configurationItems).ConfigureAwait(false);

            this.enrollments.Add((enrollingTenantId, serviceTenantId));
        }

        /// <summary>
        /// Creates a transient client tenant for the test.
        /// </summary>
        /// <returns>The new tenant.</returns>
        public async Task<ITenant> CreateTransientClientTenantAsync()
        {
            ITenant tenant =
                await this.tenantManagementService.CreateClientTenantAsync(
                    Guid.NewGuid().ToString()).ConfigureAwait(false);

            this.tenants.Add(tenant);

            if (this.primaryTransientClient == null)
            {
                this.primaryTransientClient = tenant;
            }

            return tenant;
        }

        /// <summary>
        /// Cleans up all enrollments and transient client and service tenants created using the helper.
        /// </summary>
        /// <returns>A task which completes when cleanup is finished.</returns>
        public async Task CleanupAsync()
        {
            await Task.WhenAll(
                this.enrollments.Select(
                    enrollment => this.tenantManagementService.UnenrollFromServiceAsync(
                        enrollment.EnrolledTenantId,
                        enrollment.ServiceTenantId))).ConfigureAwait(false);

            await Task.WhenAll(
                this.tenants.Select(
                    tenant => this.tenantProvider.DeleteTenantAsync(tenant.Id))).ConfigureAwait(false);
        }
    }
}
