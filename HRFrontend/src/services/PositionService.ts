import api from "./api";
import type {
  PositionReadDto,
  PositionCreateDto,
  PositionUpdateDto,
} from "../types/dto";

export async function getPositions(): Promise<PositionReadDto[]> {
  const { data } = await api.get<PositionReadDto[]>("/positions");
  return data;
}

export async function getPosition(id: number): Promise<PositionReadDto> {
  const { data } = await api.get<PositionReadDto>(`/positions/${id}`);
  return data;
}

export async function createPosition(
  dto: PositionCreateDto,
): Promise<PositionReadDto> {
  const { data } = await api.post<PositionReadDto>("/positions", dto);
  return data;
}

export async function updatePosition(
  id: number,
  dto: PositionUpdateDto,
): Promise<void> {
  await api.put(`/positions/${id}`, dto);
}

export async function deletePosition(id: number): Promise<void> {
  await api.delete(`/positions/${id}`);
}
