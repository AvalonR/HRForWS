export interface DepartmentReadDto {
  id: number;
  name: string;
  code: string;
  description: string | null;
  parentDepartmentId: number | null;
  isActive: boolean;
  createdAt: string;
  updatedAt: string | null;
}

export interface DepartmentCreateDto {
  name: string;
  code: string;
  description?: string;
  parentDepartmentId?: number;
}

export interface DepartmentUpdateDto {
  name: string;
  code: string;
  description?: string;
  parentDepartmentId?: number;
  isActive: boolean;
}

export interface PositionReadDto {
  id: number;
  title: string;
  description: string | null;
  minSalary: number | null;
  maxSalary: number | null;
  departmentId: number;
  departmentName: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string | null;
}

export interface PositionCreateDto {
  title: string;
  description?: string;
  minSalary?: number;
  maxSalary?: number;
  departmentId: number;
}

export interface PositionUpdateDto {
  title: string;
  description?: string;
  minSalary?: number;
  maxSalary?: number;
  departmentId: number;
  isActive: boolean;
}

export interface EmployeeReadDto {
  id: number;
  employeeNumber: string;
  firstName: string;
  lastName: string;
  fullName: string;
  email: string;
  phone: string | null;
  dateOfBirth: string | null;
  hireDate: string;
  terminationDate: string | null;
  address: string | null;
  city: string | null;
  state: string | null;
  postalCode: string | null;
  country: string | null;
  departmentId: number;
  departmentName: string;
  positionId: number;
  positionTitle: string;
  managerId: number | null;
  managerName: string | null;
  isActive: boolean;
  createdAt: string;
  updatedAt: string | null;
}

export interface LeaveTypeReadDto {
  id: number;
  name: string;
  daysAllowed: number;
  isPaid: boolean;
}

export interface LeaveRequestReadDto {
  id: number;
  employeeId: number;
  employeeName: string;
  leaveTypeId: number;
  leaveTypeName: string;
  startDate: string;
  endDate: string;
  status: number;
  reason: string | null;
  dateRequested: string;
  reviewedByEmployeeId: number | null;
  reviewedByEmployeeName: string | null;
  createdAt: string;
  updatedAt: string | null;
}

export interface LeaveRequestCreateDto {
  employeeId: number;
  leaveTypeId: number;
  startDate: string;
  endDate: string;
  reason?: string;
}

export interface AttendanceReadDto {
  id: number;
  employeeId: number;
  employeeName: string;
  date: string;
  checkIn: string | null;
  checkOut: string | null;
  status: number;
  notes: string | null;
  createdAt: string;
  updatedAt: string | null;
}

export interface PayrollRecordReadDto {
  id: number;
  employeeId: number;
  employeeName: string;
  payPeriodStart: string;
  payPeriodEnd: string;
  baseSalary: number;
  overtime: number;
  bonuses: number;
  deductionsTotal: number;
  netPay: number;
  payDate: string;
  status: number;
  createdAt: string;
  updatedAt: string | null;
}

export interface SalaryHistoryReadDto {
  id: number;
  employeeId: number;
  employeeName: string;
  amount: number;
  effectiveFrom: string;
  effectiveTo: string | null;
  changeReason: string | null;
  createdAt: string;
  updatedAt: string | null;
}

export interface PerformanceReviewReadDto {
  id: number;
  employeeId: number;
  employeeName: string;
  reviewerId: number;
  reviewerName: string;
  reviewDate: string;
  rating: number | null;
  strengths: string | null;
  areasForImprovement: string | null;
  goals: string | null;
  status: number;
  nextReviewDate: string | null;
  createdAt: string;
  updatedAt: string | null;
}

export interface EmployeeCreateDto {
  employeeNumber: string;
  firstName: string;
  lastName: string;
  email: string;
  phone?: string;
  dateOfBirth?: string;
  hireDate: string;
  terminationDate?: string;
  address?: string;
  city?: string;
  state?: string;
  postalCode?: string;
  country?: string;
  departmentId: number;
  positionId: number;
  managerId?: number;
}

export interface EmployeeUpdateDto {
  employeeNumber: string;
  firstName: string;
  lastName: string;
  email: string;
  phone?: string;
  dateOfBirth?: string;
  hireDate: string;
  terminationDate?: string;
  address?: string;
  city?: string;
  state?: string;
  postalCode?: string;
  country?: string;
  departmentId: number;
  positionId: number;
  managerId?: number;
  isActive: boolean;
}

export interface LeaveTypeCreateDto {
  name: string;
  daysAllowed: number;
  isPaid: boolean;
}

export interface LeaveTypeUpdateDto {
  name: string;
  daysAllowed: number;
  isPaid: boolean;
}

export interface LeaveRequestUpdateDto {
  employeeId: number;
  leaveTypeId: number;
  startDate: string;
  endDate: string;
  status: number;
  reason?: string;
  reviewedByEmployeeId?: number;
}

export interface AttendanceCreateDto {
  employeeId: number;
  date: string;
  checkIn?: string;
  checkOut?: string;
  status: number;
  notes?: string;
}

export interface AttendanceUpdateDto {
  employeeId: number;
  date: string;
  checkIn?: string;
  checkOut?: string;
  status: number;
  notes?: string;
}

export interface PayrollRecordCreateDto {
  employeeId: number;
  payPeriodStart: string;
  payPeriodEnd: string;
  baseSalary: number;
  overtime: number;
  bonuses: number;
  deductionsTotal: number;
  netPay: number;
  payDate: string;
  status: number;
}

export interface PayrollRecordUpdateDto {
  employeeId: number;
  payPeriodStart: string;
  payPeriodEnd: string;
  baseSalary: number;
  overtime: number;
  bonuses: number;
  deductionsTotal: number;
  netPay: number;
  payDate: string;
  status: number;
}

export interface SalaryHistoryCreateDto {
  employeeId: number;
  amount: number;
  effectiveFrom: string;
  effectiveTo?: string;
  changeReason?: string;
}

export interface SalaryHistoryUpdateDto {
  employeeId: number;
  amount: number;
  effectiveFrom: string;
  effectiveTo?: string;
  changeReason?: string;
}

export interface PerformanceReviewCreateDto {
  employeeId: number;
  reviewerId: number;
  reviewDate: string;
  rating?: number;
  strengths?: string;
  areasForImprovement?: string;
  goals?: string;
  status: number;
  nextReviewDate?: string;
}

export interface PerformanceReviewUpdateDto {
  employeeId: number;
  reviewerId: number;
  reviewDate: string;
  rating?: number;
  strengths?: string;
  areasForImprovement?: string;
  goals?: string;
  status: number;
  nextReviewDate?: string;
}
