import { PRIORITY_LABELS, Priority, RequestStatus, STATUS_LABELS } from '../types';

const statusClass: Record<number, string> = {
  [RequestStatus.Submitted]: 'badge-submitted',
  [RequestStatus.Assigned]: 'badge-assigned',
  [RequestStatus.InProgress]: 'badge-progress',
  [RequestStatus.Completed]: 'badge-completed',
  [RequestStatus.Rejected]: 'badge-rejected',
};

const priorityClass: Record<number, string> = {
  [Priority.Low]: 'badge-low',
  [Priority.Medium]: 'badge-medium',
  [Priority.High]: 'badge-high',
};

export function StatusBadge({ status }: { status: RequestStatus }) {
  return <span className={`badge ${statusClass[status]}`}>{STATUS_LABELS[status]}</span>;
}

export function PriorityBadge({ priority }: { priority: Priority }) {
  return <span className={`badge ${priorityClass[priority]}`}>{PRIORITY_LABELS[priority]}</span>;
}
