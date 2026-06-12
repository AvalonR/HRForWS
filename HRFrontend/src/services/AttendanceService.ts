import api from "./api";
import type {
  AttendanceReadDto,
  AttendanceCreateDto,
  AttendanceUpdateDto,
} from "../types/dto";

export async function getAttendances(): Promise<AttendanceReadDto[]> {
  const { data } = await api.get<AttendanceReadDto[]>("/attendances");
  return data;
}

export async function createAttendance(
  dto: AttendanceCreateDto,
): Promise<AttendanceReadDto> {
  const { data } = await api.post<AttendanceReadDto>("/attendances", dto);
  return data;
}

export async function updateAttendance(
  id: number,
  dto: AttendanceUpdateDto,
): Promise<void> {
  await api.put(`/attendances/${id}`, dto);
}

export async function deleteAttendance(id: number): Promise<void> {
  await api.delete(`/attendances/${id}`);
}
