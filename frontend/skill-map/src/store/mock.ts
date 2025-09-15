export const roadmaps: PlainRoadmap[] = [
  {
    id: 1,
    name: 'Frontend Developer',
    image: 'https://picsum.photos/300',
    status: 'in-progress',
  },
  {
    id: 2,
    name: 'Backend Developer',
    image: 'https://picsum.photos/300',
    status: 'completed',
  },
  {
    id: 3,
    name: 'Fullstack Developer',
    image: 'https://picsum.photos/300',
    status: 'draft',
  },
  {
    id: 4,
    name: 'Fullstack Developer',
    image: 'https://picsum.photos/300',
    status: 'draft',
  },
  {
    id: 5,
    name: 'Fullstack Developer',
    image: 'https://picsum.photos/300',
    status: 'draft',
  },
  {
    id: 6,
    name: 'Fullstack Developer',
    image: 'https://picsum.photos/300',
    status: 'draft',
  },
  {
    id: 7,
    name: 'Fullstack Developer',
    image: 'https://picsum.photos/300',
    status: 'draft',
  },
  {
    id: 8,
    name: 'Fullstack Developer',
    image: 'https://picsum.photos/300',
    status: 'draft',
  },
];

export const mockRoadmaps: Roadmap[] = [
  {
    id: 1,
    name: 'Frontend Developer',
    description: 'Learn the skills required to become a frontend engineer.',
    nodes: [
      {
        id: 1,
        title: 'HTML & CSS',
        description: 'Start with basic structure and styling of web pages.',
        //nextNodeIds: [2],
      },
      {
        id: 2,
        title: 'JavaScript Fundamentals',
        description: 'Learn variables, loops, functions, and DOM manipulation.',
        //nextNodeIds: [3, 4],
      },
      {
        id: 3,
        title: 'React.js',
        description: 'Learn modern frontend development with React.',
        // nextNodeIds: [5],
      },
      {
        id: 4,
        title: 'TypeScript',
        description: 'Add static typing to your JavaScript code.',
        // nextNodeIds: [5],
      },
      {
        id: 5,
        title: 'Frontend Tooling',
        description: 'Learn Webpack, Vite, or Next.js for building apps.',
        // nextNodeIds: [],
      },
    ],
    edges: [
      { source: 1, target: 2 },
      { source: 2, target: 3 },
      { source: 2, target: 4 },
      { source: 3, target: 5 },
      { source: 4, target: 5 },
    ],
  },
  {
    id: 2,
    name: 'Backend Developer',
    description: 'Skills and technologies for backend engineering.',
    nodes: [
      {
        id: 1,
        title: 'Programming Basics',
        description: 'Start with Python, Java, or Node.js.',
        // nextNodeIds: [2],
      },
      {
        id: 2,
        title: 'Databases',
        description: 'Learn SQL (PostgreSQL, MySQL) and NoSQL (MongoDB).',
        // nextNodeIds: [3],
      },
      {
        id: 3,
        title: 'APIs & REST',
        description: 'Build APIs and understand REST principles.',
        // nextNodeIds: [4],
      },
      {
        id: 4,
        title: 'Authentication & Security',
        description: 'JWT, OAuth, encryption, and security best practices.',
        // nextNodeIds: [5],
      },
      {
        id: 5,
        title: 'Scalability & DevOps',
        description: 'Learn Docker, CI/CD, and cloud deployment.',
        // nextNodeIds: [],
      },
    ],
    edges: [
      { source: 1, target: 2 },
      { source: 2, target: 3 },
      { source: 3, target: 4 },
      { source: 4, target: 5 },
    ],
  },
  {
    id: 3,
    name: 'Fullstack Developer',
    description:
      'Combine frontend and backend skills into fullstack expertise.',
    nodes: [
      {
        id: 1,
        title: 'Frontend Foundations',
        description: 'Cover HTML, CSS, and JavaScript basics.',
        // nextNodeIds: [2],
      },
      {
        id: 2,
        title: 'Backend Basics',
        description: 'Learn a backend language and database fundamentals.',
        // nextNodeIds: [3],
      },
      {
        id: 3,
        title: 'Frontend Framework',
        description: 'Specialize in React or Vue.',
        // nextNodeIds: [4],
      },
      {
        id: 4,
        title: 'Fullstack Projects',
        description: 'Build projects combining frontend and backend.',
        // nextNodeIds: [5],
      },
      {
        id: 5,
        title: 'Advanced Topics',
        description: 'Scalability, microservices, and testing.',
        // nextNodeIds: [],
      },
    ],
    edges: [
      { source: 1, target: 2 },
      { source: 2, target: 3 },
      { source: 3, target: 4 },
      { source: 4, target: 5 },
    ],
  },
];
