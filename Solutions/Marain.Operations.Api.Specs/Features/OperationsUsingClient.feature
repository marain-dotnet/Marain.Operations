@perFeatureContainer
@useClients
@useChildObjects

Feature: Operations

Scenario: Retrieve an operation that does not exist
	When I use the operations status client to get the operation with Id '65c9a92b-9fe8-494b-b065-0c40f033aa02'
	Then the request succeeds
	And the get operation response is null

Scenario: Create a new operation
	Given I generate a new operation Id and call it 'NewOperationId'
	When I use the operations control client to create an operation with the Id called 'NewOperationId'
	Then the request succeeds
	And the create operation response contains the location in the operations status Api for the operation with the Id called 'NewOperationId'

Scenario: Create a new operation with a resource location, expiry and body
	Given I generate a new operation Id and call it 'NewOperationId'
	When I use the operations control client to create an operation
	| IdName         | ResourceLocation       | ExpireAfter | Body                           |
	| NewOperationId | http://www.google.com/ | 300         | This is some text for the body |
	Then the request succeeds
	And the create operation response contains the location in the operations status Api for the operation with the Id called 'NewOperationId'

Scenario: Retrieve a newly created operation
	Given I generate a new operation Id and call it 'NewOperationId'
	And I use the operations control client to create an operation
	| IdName         | ResourceLocation       | ExpireAfter | Body                           |
	| NewOperationId | http://www.google.com/ | 300         | This is some text for the body |
	And the request succeeds
	When I use the operations status client to get the operation with the Id called 'NewOperationId'
	Then the request succeeds
	And the retrieved operation has the status 'NotStarted'
	And the retrieved operation has the resource location 'http://www.google.com/'
	And the retrieved operation has percent complete set to null

Scenario: Change an operation state to Running and retrieve it
	Given I generate a new operation Id and call it 'NewOperationId'
	And I use the operations control client to create an operation
	| IdName         | ResourceLocation       | ExpireAfter | Body                           |
	| NewOperationId | http://www.google.com/ | 300         | This is some text for the body |
	And I use the operations control client to set the status of the operation with Id called 'NewOperationId' to Running
	And the request succeeds
	When I use the operations status client to get the operation with the Id called 'NewOperationId'
	Then the request succeeds
	And the retrieved operation has the status 'Running'
	And the retrieved operation has percent complete set to null

Scenario: Update the percentage complete of a Running operation and retrieve it
	Given I generate a new operation Id and call it 'NewOperationId'
	And I use the operations control client to create an operation
	| IdName         | ResourceLocation       | ExpireAfter | Body                           |
	| NewOperationId | http://www.google.com/ | 300         | This is some text for the body |
	And I use the operations control client to set the status of the operation with Id called 'NewOperationId' to Running
	And the request succeeds
	And I use the operations control client to set the status of the operation with Id called 'NewOperationId' to Running and the percentage complete to 45
	And the request succeeds
	When I use the operations status client to get the operation with the Id called 'NewOperationId'
	Then the request succeeds
	And the retrieved operation has the status 'Running'
	And the retrieved operation is 45 percent complete

Scenario: Change an operation state to Succeeded and retrieve it	
	Given I generate a new operation Id and call it 'NewOperationId'
	And I use the operations control client to create an operation
	| IdName         | ResourceLocation       | ExpireAfter | Body                           |
	| NewOperationId | http://www.google.com/ | 300         | This is some text for the body |
	And I use the operations control client to set the status of the operation with Id called 'NewOperationId' to Succeeded
	And the request succeeds
	When I use the operations status client to get the operation with the Id called 'NewOperationId'
	Then the request succeeds
	And the retrieved operation has the status 'Succeeded'
	And the retrieved operation is 100 percent complete

Scenario: Change an operation state to Failed and retrieve it
	Given I generate a new operation Id and call it 'NewOperationId'
	And I use the operations control client to create an operation
	| IdName         | ResourceLocation       | ExpireAfter | Body                           |
	| NewOperationId | http://www.google.com/ | 300         | This is some text for the body |
	And I use the operations control client to set the status of the operation with Id called 'NewOperationId' to Failed
	And the request succeeds
	When I use the operations status client to get the operation with the Id called 'NewOperationId'
	Then the request succeeds
	And the retrieved operation has the status 'Failed'
	And the retrieved operation has percent complete set to null

