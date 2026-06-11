import api from "./api";
import type {
  DepartmentReadDto,
  DepartmentCreateDto,
  DepartmentUpdateDto,
} from "../types/dto";

export async function getDepartments(): Promise<DepartmentReadDto[]> {
  const { data } = await api.get<DepartmentReadDto[]>("/departments");
  return data;
}

export async function getDepartment(id: number): Promise<DepartmentReadDto> {
  const { data } = await api.get<DepartmentReadDto>(`/departments/${id}`);
  return data;
}

export async function createDepartment(
  dto: DepartmentCreateDto,
): Promise<DepartmentReadDto> {
  const { data } = await api.post<DepartmentReadDto>("/departments", dto);
  return data;
}

export async function updateDepartment(
  id: number,
  dto: DepartmentUpdateDto,
): Promise<void> {
  await api.put(`/departments/${id}`, dto);
}

export async function deleteDepartment(id: number): Promise<void> {
  await api.delete(`/departments/${id}`);
}
