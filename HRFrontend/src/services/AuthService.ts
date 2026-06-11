import type { LoginRequest, LoginResponse, CurrentUserResponse } from "../types/auth";
import api from "./api";

export async function login(data: LoginRequest): Promise<LoginResponse> {
  const response = await api.post<LoginResponse>("/auth/login", data);
  return response.data;
}

export async function getMe(): Promise<CurrentUserResponse> {
  const response = await api.get<CurrentUserResponse>("/auth/me");
  return response.data;
}

export function logout(): void {
  localStorage.removeItem("token");
  localStorage.removeItem("email");
  localStorage.removeItem("roles");
}
