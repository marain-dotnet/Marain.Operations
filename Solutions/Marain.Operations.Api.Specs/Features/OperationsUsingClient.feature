@perFeatureContainer
@useClients
@useChildObjects

Feature: Operations

Scenario: Retrieve an operation that does not exist
	When I use the operations status client to get the operation with Id '65c9a92b-9fe8-494b-b065-0c40f033aa02'
	Then then the request succeeds
	And the get operation response is null

Scenario: Create a new operation
	Given I generate a new operation Id and call it 'NewOperationId'
	When I use the operations control client to create an operation with the Id called 'NewOperationId'
	Then then the request succeeds
	And the create operation response contains the location in the operations status Api for the operation with the Id called 'NewOperationId'

Scenario: Create a new operation with a resource location, expiry and body
	Given I generate a new operation Id and call it 'NewOperationId'
	When I use the operations control client to create an operation
	| IdName         | ResourceLocation       | ExpireAfter | Body                           |
	| NewOperationId | http://www.google.com/ | 300         | This is some text for the body |
	Then then the request succeeds
	And the create operation response contains the location in the operations status Api for the operation with the Id called 'NewOperationId'

Scenario: Retrieve a newly created operation
	Given I generate a new operation Id and call it 'NewOperationId'
	When I use the operations control client to create an operation
	| IdName         | ResourceLocation       | ExpireAfter | Body                           |
	| NewOperationId | http://www.google.com/ | 300         | This is some text for the body |
	When I use the operations status client to get the operation with the Id called 'NewOperationId'
	Then then the request succeeds
	And the retrieved operation has the status 'NotStarted'
	And the retrieved operation has the resource location 'http://www.google.com/'
	And the retrieved operation has percent complete set to null

Scenario: Change an operation state to Running and retrieve it


Scenario: Change an operation state to Complete and retrieve it	


Scenario: Change an operation state to Failed and retrieve it

