import api from "./api";
import type {
  LeaveRequestReadDto,
  LeaveRequestCreateDto,
  LeaveRequestUpdateDto,
} from "../types/dto";

export async function getLeaveRequests(): Promise<LeaveRequestReadDto[]> {
  const { data } = await api.get<LeaveRequestReadDto[]>("/leaverequests");
  return data;
}

export async function getLeaveRequest(
  id: number,
): Promise<LeaveRequestReadDto> {
  const { data } = await api.get<LeaveRequestReadDto>(`/leaverequests/${id}`);
  return data;
}

export async function createLeaveRequest(
  dto: LeaveRequestCreateDto,
): Promise<LeaveRequestReadDto> {
  const { data } = await api.post<LeaveRequestReadDto>(
    "/leaverequests",
    dto,
  );
  return data;
}

export async function updateLeaveRequest(
  id: number,
  dto: LeaveRequestUpdateDto,
): Promise<void> {
  await api.put(`/leaverequests/${id}`, dto);
}

export async function deleteLeaveRequest(id: number): Promise<void> {
  await api.delete(`/leaverequests/${id}`);
}
