export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  email: string;
  roles: string[];
}

export interface CurrentUserResponse {
  email: string;
  roles: string[];
  employeeId: number | null;
}
