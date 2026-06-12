import api from "./api";
import type {
  PerformanceReviewReadDto,
  PerformanceReviewCreateDto,
  PerformanceReviewUpdateDto,
} from "../types/dto";

export async function getPerformanceReviews(): Promise<
  PerformanceReviewReadDto[]
> {
  const { data } = await api.get<PerformanceReviewReadDto[]>(
    "/performancereviews",
  );
  return data;
}

export async function createPerformanceReview(
  dto: PerformanceReviewCreateDto,
): Promise<PerformanceReviewReadDto> {
  const { data } = await api.post<PerformanceReviewReadDto>(
    "/performancereviews",
    dto,
  );
  return data;
}

export async function updatePerformanceReview(
  id: number,
  dto: PerformanceReviewUpdateDto,
): Promise<void> {
  await api.put(`/performancereviews/${id}`, dto);
}

export async function deletePerformanceReview(
  id: number,
): Promise<void> {
  await api.delete(`/performancereviews/${id}`);
}
