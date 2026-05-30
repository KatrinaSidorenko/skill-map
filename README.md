# 🗺️ SkillMap

> **Take control of your learning journey.** 🚀

SkillMap is a dynamic web application designed to help users map out, visualize, and master their educational goals. By modeling learning paths as **Directed Acyclic Graphs (DAGs)**, where each node represents a topic or subtopic, learners can effortlessly explore existing trajectories, customize them on the fly, or build their own masterclass paths from scratch.

With a built-in knowledge-testing module, SkillMap tracks real-time progress and delivers smart, structural recommendations driven entirely by the learner's unique position within the graph.

---

## ✨ Key Features

* 🔐 **Auth & Profile** — Secure registration, login, and a fully editable user profile to track personal growth.
* 🧭 **Trajectory Browser** — Discover, save, and fork existing community learning paths to jumpstart your education.
* 📊 **Visual Graph Editor** — Design and modify learning trajectories visually as DAGs. Nodes are split into **Topics** or **Subtopics**, each storing a title, description, progress status, and curated resource links (courses, websites, or videos). The intuitive editor supports seamless node and edge creation/deletion.
* 🎨 **Custom Workspace** — Prefer a clean slate? Create a blank canvas and engineer a highly personalized learning trajectory from the ground up.
* 🧠 **Knowledge Testing** — Dynamically generate quizzes based on the current state of your trajectory graph to validate what you've actually retained.
* 📈 **Graph-Driven Recommendations** — No generic algorithms here. Your position on the graph is determined by node completion statuses. After finishing a test, SkillMap reviews the graph's topology to recommend the precise next steps to advance your path.

---

## 🛠️ Tech Stack

### 🖥️ Backend

* **Core Framework:** ASP.NET Core & C#
* **Data Access:** Entity Framework Core
* 🔌 **Real-Time Communication:** SignalR (Enables live, bi-directional updates and validations for graph editor)
* **🧪 Testing Suite:**
* `xUnit` (Unit & Integration testing)
* `Moq` (Mocking framework)
* `Bogus` (Fake data generation)
* `Testcontainers` (Disposable Docker containers for integration tests)
* `FluentAssertions` (Expressive assertion syntax)
* `k6` (Load testing for performance validation)



### 🗄️ Databases & Event Streaming

* 🐘 **PostgreSQL** — Handles structured relational data and user profiles.
* 🕸️ **Neo4j** — Powers the graph database structure for seamless DAG traversal and recommendations.
* 💬 **Apache Kafka** — Manages high-throughput event streaming for robust backend data pipelines.

### 🎨 Frontend

* **Core Framework:** Next.js & TypeScript
* 🔌 **Real-Time Communication:** SignalR Client (WebSockets integration for immediate UI state synchronization)
* **UI Library:** Chakra UI
* **Graph Engine:** React Flow (Powers the interactive visualization editor)
* **State Management:** Redux Toolkit
* **🌐 Internationalization:** `next-intl`

### 🐋 Deployment

* **Containerization:** Docker