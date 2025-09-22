interface AppUser {
  id: string;
  email: string;
  username: string;
  avatarUrl?: string;
}

type Role = 'user' | 'mentor' | 'admin';

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
