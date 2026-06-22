interface AppUser {
  id: string;
  email: string;
  username: string;
  imageUrl?: string;
}

type Role = 'User' | 'Mentor' | 'Admin';

interface RegistrationRequest {
  email: string;
  password: string;
  username: string;
  role: Role;
}

interface LoginRequest {
  email: string;
  password: string;
}

interface AuthResponse {
  token: string;
  user: AppUser;
}

interface SetNewPasswordRequest {
  token: string;
  password: string;
}

interface PasswordResetRequest {
  email: string;
}

interface UpdateProfileRequest {
  username?: string;
  email?: string;
  imageUrl?: string;
}

interface DashboardStats {
  savedRoadmaps: number;
  activeRoadmaps: number;
  testsCompleted: number;
  averageScore: number;
}

interface InProgressRoadmap {
  workspaceId: string;
  title: string;
  description?: string;
  imageUrl?: string;
  progress: number;
  savedAt: string;
  status: string;
  totalNodes: number;
}

interface RecentAssessmentAttempt {
  attemptId: string;
  assessmentId: string;
  title: string;
  type: string;
  score: number | null;
  maxScore: number;
  startedAt: string;
  completedAt: string | null;
  status: string;
}

interface UserDashboard {
  stats: DashboardStats;
  inProgressRoadmaps: InProgressRoadmap[];
  recentTests: RecentAssessmentAttempt[];
}
