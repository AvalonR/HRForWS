import api from "./api";
import type {
  EmployeeReadDto,
  EmployeeCreateDto,
  EmployeeUpdateDto,
} from "../types/dto";

export async function getEmployees(): Promise<EmployeeReadDto[]> {
  const { data } = await api.get<EmployeeReadDto[]>("/employees");
  return data;
}

export async function getEmployee(id: number): Promise<EmployeeReadDto> {
  const { data } = await api.get<EmployeeReadDto>(`/employees/${id}`);
  return data;
}

export async function createEmployee(
  dto: EmployeeCreateDto,
): Promise<EmployeeReadDto> {
  const { data } = await api.post<EmployeeReadDto>("/employees", dto);
  return data;
}

export async function updateEmployee(
  id: number,
  dto: EmployeeUpdateDto,
): Promise<void> {
  await api.put(`/employees/${id}`, dto);
}

export async function deleteEmployee(id: number): Promise<void> {
  await api.delete(`/employees/${id}`);
}
