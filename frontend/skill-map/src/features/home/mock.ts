export const mockDashboardStats = {
  savedRoadmaps: 5,
  testsCompleted: 12,
  averageScore: 76,
  activeRoadmaps: 3,
};

export const mockInProgressRoadmaps: SavedPlainRoadmap[] = [
  {
    id: '1',
    workspaceId: 'ws1',
    title: 'React Development Path',
    description: 'Learn React from basics to advanced patterns.',
    imageUrl: '',
    progress: 65,
    savedAt: '2025-01-01T00:00:00Z',
    status: 'inprogress',
    totalNodes: 20,
  },
  {
    id: '2',
    workspaceId: 'ws1',
    title: 'System Design Fundamentals',
    description: 'Master scalable system design principles.',
    imageUrl: '',
    progress: 30,
    savedAt: '2024-12-15T00:00:00Z',
    status: 'inprogress',
    totalNodes: 15,
  },
  {
    id: '3',
    workspaceId: 'ws1',
    title: 'Data Structures & Algorithms',
    description: 'Deep dive into DSA concepts and techniques.',
    imageUrl: '',
    progress: 90,
    savedAt: '2024-11-20T00:00:00Z',
    status: 'inprogress',
    totalNodes: 30,
  },
];

export interface RecentTest {
  testId: string;
  type: string;
  score: number | null;
  maxScore: number;
  completedAt: string | null;
  status: 'completed' | 'in_progress';
}

export const mockRecentTests: RecentTest[] = [
  {
    attemptId: 'algorithms-basic',
    type: 'Algorithms – Basics',
    score: 88,
    maxScore: 100,
    completedAt: '2025-01-05T14:52:00Z',
    status: 'completed',
  },
  {
    attemptId: 'system-design-intro',
    type: 'System Design – Intro',
    score: 71,
    maxScore: 80,
    completedAt: '2025-01-03T09:20:00Z',
    status: 'completed',
  },
  {
    attemptId: 'data-structures-intermediate',
    type: 'Data Structures – Intermediate',
    score: null,
    maxScore: 120,
    completedAt: null,
    status: 'in_progress',
  },
  {
    attemptId: 'react-fundamentals',
    type: 'React – Fundamentals',
    score: 55,
    maxScore: 100,
    completedAt: '2024-12-28T11:40:00Z',
    status: 'completed',
  },
];
