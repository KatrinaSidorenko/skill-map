In workspace each change of user is an event that is stored in db. 

For now it look like:
1. User changes something in the UI
2. UI sends request to backend to commit the change (rach event has a unique id)
3. Backend receives the request, creates a new event and stores it in db with version number = last version number + 1
    if event is of type edge created => the event should be checked
4. Backend sends response to UI with 202 Accepted
5. UI in background fetches the events from backend and updates the UI accordingly

Problems:
1. Problem in version update, when two events are attempted to insert at the same time
2. Background processing is parallel that can lead to inconsistent graph loops check
3. An order of events
4. Add events returning with Accept and storing in UI

Possible solutions:
1. Use transactions properites in db. here is already exesit unique index on version number and workspace id, so if two events are attempted to insert at the same time, one of them will fail with unique constraint violation. In this case, the backend should catch this exception and retry the operation by fetching the latest version number and trying to insert again.

2. If we introduce the queue for processing events, we can ensure that events are processed in the order they were created. This way, we can avoid the race condition and ensure that the graph loops check is consistent. 

How it can look like:

1. User load the workspace -> Featch the snapshots and events and compose the graph. Last event version is the version of the workspace.
2. User change something in workspace -> 
    Client should:
    2.1 Create a new event with unique id and version number = last version number + 1
    2.2 Send the event to the backend for processing
    2.3 Client put the event in the local queue for processing and wait for the response from the backend

    Backend should:
    2.1 Receive the event and put it in the processing queue
    2.2 Send response to the client with 202 Accepted

3. Background processing:
    3.1 The backend process the events from the queue one by one (it is better that the queue is persistent, so in case of backend crash, we won't lose the events + same process process the events of same workspace, so we can ensure the order of events)
    3.2 For each event, the backend should check if the version number of the event is equal to the last version number of the workspace + 1. If not, it means that there is a race condition

    What if in client can be made changes in batches? And how it can be managed if several clients are making changes in the same workspace at the same time (for future consideration)?

4. Client poll the backend for statuses of the events in the local queue. If the event is processed successfully, the client can remove it from the local queue and update the UI accordingly. If the event processing failed, the client can retry sending the event to the backend or show an error message to the user.