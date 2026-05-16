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
