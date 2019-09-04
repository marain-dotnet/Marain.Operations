@status
@setupContainer
Feature: OperationsStatusApiAndTasks
	In order to enable observation of long-running operations
	As a developer
	I want to verify that the OperationsStatusService and OperationsStatusTasks types work together as expected

Scenario: Get non-existent operation
	Given There is no operation in the store with id 'd306cb37-bc58-40fc-801c-bce5fb2c3a67'
	When I call OperationsStatusOpenApiService.GetOperationById with id 'd306cb37-bc58-40fc-801c-bce5fb2c3a67'
	Then the result status should be 404

Scenario: Get not started operation
	Given There is an operation in the store with id 'd306cb37-bc58-40fc-801c-bce5fb2c3a67' and a status of 'NotStarted'
	When I call OperationsStatusOpenApiService.GetOperationById with id 'd306cb37-bc58-40fc-801c-bce5fb2c3a67'
	Then the result status should be 200
    And the operation status in the result should be 'NotStarted'

Scenario: Get running operation
	Given There is a running operation in the store with id 'd306cb37-bc58-40fc-801c-bce5fb2c3a67' and a percentComplete of 42
	When I call OperationsStatusOpenApiService.GetOperationById with id 'd306cb37-bc58-40fc-801c-bce5fb2c3a67'
	Then the result status should be 200
    And the operation status in the result should be 'Running'
    And the percentComplete in the result should be 42
