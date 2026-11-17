export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface RegisterRequest {
  username: string;
  password: string;
  fullName: string;
}

export interface AuthResponse {
  token: string;
  username: string;
  fullName: string;
  role: string;
  expiresAt: string;
}

export interface UserProfile {
  id: number;
  username: string;
  fullName: string;
  email?: string;
  avatarUrl?: string;
  role: string;
  hasPassword: boolean;
  createdAt: string;
}

export interface UpdateProfileRequest {
  fullName: string;
  email?: string;
}

export interface ChangePasswordRequest {
  currentPassword?: string;
  newPassword: string;
  confirmPassword: string;
}

export interface LinkedAccountDto {
  provider: string; // 'Google' | 'Facebook' | 'GitHub'
  isLinked: boolean;
  providerEmail?: string;
  providerDisplayName?: string;
  linkedAt?: string;
}
