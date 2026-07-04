import { api } from './client';
import type {
  AssignmentDto,
  AuthResponse,
  CategoryDto,
  PagedResult,
  ReportSummary,
  ServiceRequestDto,
  StatusUpdateDto,
  UserDto,
} from '../types';

// ---- Auth ----
export const authApi = {
  login: (email: string, password: string) =>
    api.post<AuthResponse>('/auth/login', { email, password }).then((r) => r.data),
  register: (firstName: string, lastName: string, email: string, password: string) =>
    api.post<AuthResponse>('/auth/register', { firstName, lastName, email, password }).then((r) => r.data),
};

// ---- Categories ----
export const categoriesApi = {
  getAll: () => api.get<CategoryDto[]>('/categories').then((r) => r.data),
  create: (name: string, description?: string) =>
    api.post<CategoryDto>('/categories', { name, description }).then((r) => r.data),
  update: (id: string, name: string, description?: string) =>
    api.put<CategoryDto>(`/categories/${id}`, { name, description }).then((r) => r.data),
  remove: (id: string) => api.delete(`/categories/${id}`).then((r) => r.data),
};

// ---- Service requests ----
export interface RequestQuery {
  search?: string;
  status?: number;
  categoryId?: string;
  page?: number;
  pageSize?: number;
}

export const requestsApi = {
  getPaged: (query: RequestQuery) =>
    api
      .get<PagedResult<ServiceRequestDto>>('/service-requests', { params: query })
      .then((r) => r.data),
  getById: (id: string) =>
    api.get<ServiceRequestDto>(`/service-requests/${id}`).then((r) => r.data),
  getHistory: (id: string) =>
    api.get<StatusUpdateDto[]>(`/service-requests/${id}/history`).then((r) => r.data),
  create: (form: FormData) =>
    api
      .post<ServiceRequestDto>('/service-requests', form, {
        headers: { 'Content-Type': 'multipart/form-data' },
      })
      .then((r) => r.data),
  updateStatus: (id: string, status: number, comment?: string) =>
    api
      .patch<ServiceRequestDto>(`/service-requests/${id}/status`, { status, comment })
      .then((r) => r.data),
  remove: (id: string) => api.delete(`/service-requests/${id}`).then((r) => r.data),
};

// ---- Assignments ----
export const assignmentsApi = {
  assign: (serviceRequestId: string, officerId: string, notes?: string) =>
    api
      .post<AssignmentDto>('/assignments', { serviceRequestId, officerId, notes })
      .then((r) => r.data),
};

// ---- Users (admin) ----
export const usersApi = {
  getPaged: (search: string, page: number, pageSize: number) =>
    api
      .get<PagedResult<UserDto>>('/users', { params: { search, page, pageSize } })
      .then((r) => r.data),
  getOfficers: () => api.get<UserDto[]>('/users/officers').then((r) => r.data),
  create: (payload: {
    firstName: string;
    lastName: string;
    email: string;
    password: string;
    role: string;
  }) => api.post<UserDto>('/users', payload).then((r) => r.data),
  remove: (id: string) => api.delete(`/users/${id}`).then((r) => r.data),
};

// ---- Reports (admin) ----
export const reportsApi = {
  summary: () => api.get<ReportSummary>('/reports/summary').then((r) => r.data),
};
