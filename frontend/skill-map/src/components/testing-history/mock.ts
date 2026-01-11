export const testingHistoryMock: TestingHistoryDto = {
  items: [
    {
      testId: 'algorithms-basic',
      type: 'Algorithms – Basics',
      maxScore: 100,
      attempts: [
        {
          resultId: 'alg-001',
          startedAt: '2025-01-02T09:15:00Z',
          completedAt: '2025-01-02T09:45:00Z',
          score: 72,
        },
        {
          resultId: 'alg-002',
          startedAt: '2025-01-05T14:10:00Z',
          completedAt: '2025-01-05T14:52:00Z',
          score: 88,
        },
      ],
    },

    {
      testId: 'data-structures-intermediate',
      type: 'Data Structures – Intermediate',
      maxScore: 120,
      attempts: [
        {
          resultId: 'ds-001',
          startedAt: '2025-01-07T11:30:00Z',
          completedAt: null, // in progress
          score: null,
        },
      ],
    },

    {
      testId: 'system-design-intro',
      type: 'System Design – Intro',
      maxScore: 80,
      attempts: [
        {
          resultId: 'sd-001',
          startedAt: '2024-12-20T10:00:00Z',
          completedAt: '2024-12-20T10:35:00Z',
          score: 44,
        },
        {
          resultId: 'sd-002',
          startedAt: '2024-12-23T16:05:00Z',
          completedAt: '2024-12-23T16:42:00Z',
          score: 63,
        },
        {
          resultId: 'sd-003',
          startedAt: '2025-01-03T08:50:00Z',
          completedAt: '2025-01-03T09:20:00Z',
          score: 71,
        },
      ],
    },

    {
      testId: 'typescript-advanced',
      type: 'TypeScript – Advanced',
      maxScore: 90,
      attempts: [], // no attempts yet
    },
  ],
};