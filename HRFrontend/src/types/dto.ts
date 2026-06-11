export interface DepartmentReadDto {
  id: number;
  name: string;
  code: string;
  description: string | null;
  parentDepartmentId: number | null;
  isActive: boolean;
  createdAt: string; // ISO 8601
  updatedAt: string | null; // ISO 8601
}

export interface DepartmentCreateDto {
  name: string; // required, max 100
  code: string; // required, max 20
  description?: string; // max 500
  parentDepartmentId?: number;
}

export interface DepartmentUpdateDto {
  name: string; // required, max 100
  code: string; // required, max 20
  description?: string; // max 500
  parentDepartmentId?: number;
  isActive: boolean;
}
