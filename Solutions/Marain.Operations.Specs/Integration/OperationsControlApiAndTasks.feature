@operationsControl
@setupContainer
Feature: OperationsControlApiAndTasks
	In order to enable observation of long-running operations
	As a developer
	I want to verify that the OperationsStatusService and OperationsStatusTasks types work together as expected

Scenario: Create non-existent operation
	Given There is no operation in the store with id 'd306cb37-bc58-40fc-801c-bce5fb2c3a67'
	When I call OperationsControlOpenApiService.CreateOperation with id 'd306cb37-bc58-40fc-801c-bce5fb2c3a67'
	Then the result status should be 201
    And the 'Location' property in the result should be 'http://operationsstatus.example.com/RootTenant/api/operations/d306cb37-bc58-40fc-801c-bce5fb2c3a67'

Scenario: Change not started operation to failed
	Given There is an operation in the store with id 'd306cb37-bc58-40fc-801c-bce5fb2c3a67' and a status of 'NotStarted'
	When I call OperationsStatusOpenApiService.SetOperationFailed with id 'd306cb37-bc58-40fc-801c-bce5fb2c3a67'
	Then the result status should be 201
    And the 'Location' property in the result should be 'http://operationsstatus.example.com/RootTenant/api/operations/d306cb37-bc58-40fc-801c-bce5fb2c3a67'
    And the status of the operation in the store with id 'd306cb37-bc58-40fc-801c-bce5fb2c3a67' should be 'Failed'

Scenario: Change not started operation to running
	Given There is an operation in the store with id 'd306cb37-bc58-40fc-801c-bce5fb2c3a67' and a status of 'NotStarted'
	When I call OperationsStatusOpenApiService.SetOperationRunning with id 'd306cb37-bc58-40fc-801c-bce5fb2c3a67' and percentComplete of 24
	Then the result status should be 201
    And the 'Location' property in the result should be 'http://operationsstatus.example.com/RootTenant/api/operations/d306cb37-bc58-40fc-801c-bce5fb2c3a67'
    And the status of the operation in the store with id 'd306cb37-bc58-40fc-801c-bce5fb2c3a67' should be 'Running'
    And the percentComplete of the operation in the store with id 'd306cb37-bc58-40fc-801c-bce5fb2c3a67' should be 24

Scenario: Change running operation to succeeded
	Given There is an operation in the store with id 'd306cb37-bc58-40fc-801c-bce5fb2c3a67' and a status of 'Running'
	When I call OperationsStatusOpenApiService.SetOperationSucceeded with id 'd306cb37-bc58-40fc-801c-bce5fb2c3a67'
	Then the result status should be 201
    And the 'Location' property in the result should be 'http://operationsstatus.example.com/RootTenant/api/operations/d306cb37-bc58-40fc-801c-bce5fb2c3a67'
    And the status of the operation in the store with id 'd306cb37-bc58-40fc-801c-bce5fb2c3a67' should be 'Succeeded'
