# Release notes for Marain.Operations v3.

## v3.0

V3-style Service Manifest
Configuration keys for V3-style tenant storage configuration settings now in line with conventions.

### Breaking changes

This changes the keys under which we expect V3-style storage configuration to be stored in tenant
properties.

We are not aware of any deployments that use the new V3-style storage configuration (not least
because we never created an updated service manifest, and until recently the tenant management
tooling was only able to create v2-style entries). However, since it's technically possible that
someone could manually have created suitable configuration entries, this is a breaking change so
we bumped the major version number.

Note that the service tenant ID remains unchanged and the service name is still Claims V1 because
we continue to support the old-style configuration. So although upgrading from v2-v3 has the
potential to be a breaking change, upgrading from v1-v3 will be fine.