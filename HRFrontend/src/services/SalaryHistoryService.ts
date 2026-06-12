import api from "./api";
import type {
  SalaryHistoryReadDto,
  SalaryHistoryCreateDto,
  SalaryHistoryUpdateDto,
} from "../types/dto";

export async function getSalaryHistories(): Promise<SalaryHistoryReadDto[]> {
  const { data } = await api.get<SalaryHistoryReadDto[]>("/salaryhistories");
  return data;
}

export async function createSalaryHistory(
  dto: SalaryHistoryCreateDto,
): Promise<SalaryHistoryReadDto> {
  const { data } = await api.post<SalaryHistoryReadDto>(
    "/salaryhistories",
    dto,
  );
  return data;
}

export async function updateSalaryHistory(
  id: number,
  dto: SalaryHistoryUpdateDto,
): Promise<void> {
  await api.put(`/salaryhistories/${id}`, dto);
}

export async function deleteSalaryHistory(id: number): Promise<void> {
  await api.delete(`/salaryhistories/${id}`);
}
