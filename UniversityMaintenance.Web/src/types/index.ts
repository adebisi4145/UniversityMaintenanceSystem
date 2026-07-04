// ---- Enums (numeric, matching the API's System.Text.Json serialization) ----
// Modelled as const objects + union types because the project's tsconfig enables
// `erasableSyntaxOnly`, which disallows TS `enum` declarations.
export const Priority = {
  Low: 0,
  Medium: 1,
  High: 2,
} as const;
export type Priority = (typeof Priority)[keyof typeof Priority];

export const RequestStatus = {
  Submitted: 0,
  Assigned: 1,
  InProgress: 2,
  Completed: 3,
  Rejected: 4,
} as const;
export type RequestStatus = (typeof RequestStatus)[keyof typeof RequestStatus];

export const PRIORITY_LABELS: Record<number, string> = {
  [Priority.Low]: 'Low',
  [Priority.Medium]: 'Medium',
  [Priority.High]: 'High',
};

export const STATUS_LABELS: Record<number, string> = {
  [RequestStatus.Submitted]: 'Submitted',
  [RequestStatus.Assigned]: 'Assigned',
  [RequestStatus.InProgress]: 'In Progress',
  [RequestStatus.Completed]: 'Completed',
  [RequestStatus.Rejected]: 'Rejected',
};

export const Roles = {
  Student: 'Student',
  Officer: 'Officer',
  Admin: 'Admin',
} as const;
export type Role = (typeof Roles)[keyof typeof Roles];

// ---- DTOs ----
export interface UserDto {
  id: string;
  firstName: string;
  lastName: string;
  fullName: string;
  email: string;
  role: string;
  createdAt: string;
}

export interface AuthResponse {
  token: string;
  expiresAt: string;
  user: UserDto;
}

export interface CategoryDto {
  id: string;
  name: string;
  description?: string | null;
}

export interface ServiceRequestDto {
  id: string;
  title: string;
  description: string;
  location: string;
  priority: Priority;
  status: RequestStatus;
  evidenceImagePath?: string | null;
  categoryId: string;
  categoryName: string;
  requesterId: string;
  requesterName: string;
  assignedOfficerId?: string | null;
  assignedOfficerName?: string | null;
  createdAt: string;
  updatedAt?: string | null;
}

export interface StatusUpdateDto {
  id: string;
  oldStatus: RequestStatus;
  newStatus: RequestStatus;
  changedById: string;
  changedByName: string;
  comment?: string | null;
  createdAt: string;
}

export interface AssignmentDto {
  id: string;
  serviceRequestId: string;
  serviceRequestTitle: string;
  officerId: string;
  officerName: string;
  assignedByAdminId: string;
  assignedAt: string;
  notes?: string | null;
}

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPrevious: boolean;
  hasNext: boolean;
}

export interface ReportSummary {
  totalRequests: number;
  totalUsers: number;
  requestsByStatus: Record<string, number>;
  requestsByCategory: Record<string, number>;
  requestsByPriority: Record<string, number>;
}
