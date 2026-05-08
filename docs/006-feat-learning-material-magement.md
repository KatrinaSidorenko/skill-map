The task is to add ability to manage learnign materials\resources to the learning item like (topic or subtopic).

Here are two approaches that i see:
1. Represent them as a part of the graph
    1.1 It can lead to more vomplex visualization and more complex graph management
    1.2 It requires judicios changes in all other parts of the system where the code relays on fact that it consists only from topics and subtopics

2. Respresent them as list of resources in the sidebar menu of the node
    2.1 It is more simple to implement and maintain
    2.2 It does not require changes in the graph structure and visualization
    2.3 It allows to easily add different types of resources (links, files, videos, etc.) without affecting the graph structure

But here is the point of storage. For now, recommended resources are stored in the graph based storage, when user saves it we need to pick the graph and create the snapshot of it. 
So how to store the learning materuals? As part of snapshot? that leads to more complexity. Or as separate table in relation db where are copid all resources to the workspace like (learningItemId, resources_id, type, url, title, description, etc.)? 

But maybe such approach with copying resources to the workspace is not good, because it can lead to data inconsistency and duplication. 