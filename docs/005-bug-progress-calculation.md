### Problem
The progress now is calculated as the realtion of count of vertivies with completed status to the total count of verticies. The problem is that the progress of the topic is derived from its subtopics.

The nodes projection is stored in separate table and the inforamation about the node type isn't stored.

### Solution
Add the node type into projection table
Fix the calculation logic so that the progress is based on active subtopics with completed status / all active subtopics. 