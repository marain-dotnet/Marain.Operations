@perFeatureContainer
@useClients

Feature: Operations

Scenario: Retrieve an operation that does not exist
	When I use the operations status client to get the operation with Id '65c9a92b-9fe8-494b-b065-0c40f033aa02'
	Then an exception of type 'HttpOperationException' is thrown

Scenario: Retrieve a newly created operation


Scenario: Change an operation state to Running and retrieve it


Scenario: Change an operation state to Complete and retrieve it	


Scenario: Change an operation state to Failed and retrieve it

