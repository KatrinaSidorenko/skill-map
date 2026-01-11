interface TestingHistoryDto {
  items: TestHistoryItemDto[];
}

interface TestHistoryItemDto {
  testId: string;
  maxScore: number;
  type: string;
  attempts: TestAttemptDto[];
}

interface TestAttemptDto {
  resultId: string;
  startedAt: string; // ISO date string
  completedAt: string | null; // ISO date string or null
  score: number | null;
}
