# SkillMap

> Structure your learning path.

SkillMap is a web application that helps users take control of their
learning journey. It models learning paths as directed acyclic graphs (DAGs), where each node represents a topic or subtopic, allowing learners to explore existing trajectories, customize them, or build their own from scratch. A
built-in knowledge-testing module tracks progress and delivers recommendations
driven by the learner's position within the graph.

---

## Features

- **Auth & profile** — Registration, login, and an editable user profile.
- **Trajectory browser** — Browse, save, and fork existing learning paths.
- **Graph editor** — Edit learning trajectories visually as DAGs. Nodes are of
  two types — *topic* or *subtopic* — and each stores a title, description,
  status, and optional learning resources (links to courses, websites, or
  videos). The editor supports adding and removing nodes and edges.
- **Custom workspace** — Create a blank canvas and design a personal learning
  trajectory from scratch.
- **Knowledge testing** — Generate a test based on the current state of a
  trajectory graph and receive recommendations on where to go next.
- **Graph-driven recommendations** — A learner's position on the graph is
  determined by the status of each node. After completing a test, recommendations
  are derived from the graph structure itself and can be applied fully or
  partially to advance along the path.

---
