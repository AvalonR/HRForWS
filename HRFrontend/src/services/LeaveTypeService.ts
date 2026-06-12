import api from "./api";
import type {
  LeaveTypeReadDto,
  LeaveTypeCreateDto,
  LeaveTypeUpdateDto,
} from "../types/dto";

export async function getLeaveTypes(): Promise<LeaveTypeReadDto[]> {
  const { data } = await api.get<LeaveTypeReadDto[]>("/leavetypes");
  return data;
}

export async function createLeaveType(
  dto: LeaveTypeCreateDto,
): Promise<LeaveTypeReadDto> {
  const { data } = await api.post<LeaveTypeReadDto>("/leavetypes", dto);
  return data;
}

export async function updateLeaveType(
  id: number,
  dto: LeaveTypeUpdateDto,
): Promise<void> {
  await api.put(`/leavetypes/${id}`, dto);
}

export async function deleteLeaveType(id: number): Promise<void> {
  await api.delete(`/leavetypes/${id}`);
}
