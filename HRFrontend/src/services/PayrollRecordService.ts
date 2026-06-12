import api from "./api";
import type {
  PayrollRecordReadDto,
  PayrollRecordCreateDto,
  PayrollRecordUpdateDto,
} from "../types/dto";

export async function getPayrollRecords(): Promise<PayrollRecordReadDto[]> {
  const { data } = await api.get<PayrollRecordReadDto[]>("/payrollrecords");
  return data;
}

export async function createPayrollRecord(
  dto: PayrollRecordCreateDto,
): Promise<PayrollRecordReadDto> {
  const { data } = await api.post<PayrollRecordReadDto>(
    "/payrollrecords",
    dto,
  );
  return data;
}

export async function updatePayrollRecord(
  id: number,
  dto: PayrollRecordUpdateDto,
): Promise<void> {
  await api.put(`/payrollrecords/${id}`, dto);
}

export async function deletePayrollRecord(id: number): Promise<void> {
  await api.delete(`/payrollrecords/${id}`);
}
