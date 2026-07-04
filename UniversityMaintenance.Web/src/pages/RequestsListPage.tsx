import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { errorMessage } from '../api/client';
import { categoriesApi, requestsApi } from '../api/endpoints';
import { useAuth } from '../auth/AuthContext';
import { Pagination } from '../components/Pagination';
import { PriorityBadge, StatusBadge } from '../components/StatusBadge';
import { useToast } from '../components/Toast';
import {
  Roles,
  STATUS_LABELS,
  type CategoryDto,
  type PagedResult,
  type ServiceRequestDto,
} from '../types';

export function RequestsListPage() {
  const { hasRole } = useAuth();
  const toast = useToast();
  const [data, setData] = useState<PagedResult<ServiceRequestDto> | null>(null);
  const [categories, setCategories] = useState<CategoryDto[]>([]);
  const [loading, setLoading] = useState(true);

  const [search, setSearch] = useState('');
  const [status, setStatus] = useState<string>('');
  const [categoryId, setCategoryId] = useState<string>('');
  const [page, setPage] = useState(1);

  const title = hasRole(Roles.Student)
    ? 'My Requests'
    : hasRole(Roles.Officer)
      ? 'Assigned Jobs'
      : 'All Requests';

  useEffect(() => {
    categoriesApi.getAll().then(setCategories).catch(() => undefined);
  }, []);

  useEffect(() => {
    setLoading(true);
    requestsApi
      .getPaged({
        search: search || undefined,
        status: status === '' ? undefined : Number(status),
        categoryId: categoryId || undefined,
        page,
        pageSize: 8,
      })
      .then(setData)
      .catch((err) => toast.error(errorMessage(err)))
      .finally(() => setLoading(false));
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [search, status, categoryId, page]);

  return (
    <div className="page">
      <header className="page-head">
        <h1>{title}</h1>
        {hasRole(Roles.Student) && (
          <Link to="/requests/new" className="btn btn-primary">
            + New Request
          </Link>
        )}
      </header>

      <div className="filters card">
        <input
          className="filter-search"
          placeholder="Search title, description, location…"
          value={search}
          onChange={(e) => {
            setPage(1);
            setSearch(e.target.value);
          }}
        />
        <select
          value={status}
          onChange={(e) => {
            setPage(1);
            setStatus(e.target.value);
          }}
        >
          <option value="">All statuses</option>
          {Object.entries(STATUS_LABELS).map(([value, label]) => (
            <option key={value} value={value}>
              {label}
            </option>
          ))}
        </select>
        <select
          value={categoryId}
          onChange={(e) => {
            setPage(1);
            setCategoryId(e.target.value);
          }}
        >
          <option value="">All categories</option>
          {categories.map((c) => (
            <option key={c.id} value={c.id}>
              {c.name}
            </option>
          ))}
        </select>
      </div>

      <div className="card">
        {loading ? (
          <p className="muted">Loading…</p>
        ) : !data || data.items.length === 0 ? (
          <p className="muted empty">No requests match your filters.</p>
        ) : (
          <div className="table-wrap">
            <table className="table">
              <thead>
                <tr>
                  <th>Title</th>
                  <th>Category</th>
                  <th>Location</th>
                  <th>Priority</th>
                  <th>Status</th>
                  {!hasRole(Roles.Student) && <th>Requester</th>}
                  <th></th>
                </tr>
              </thead>
              <tbody>
                {data.items.map((r) => (
                  <tr key={r.id}>
                    <td className="strong">{r.title}</td>
                    <td>{r.categoryName}</td>
                    <td>{r.location}</td>
                    <td>
                      <PriorityBadge priority={r.priority} />
                    </td>
                    <td>
                      <StatusBadge status={r.status} />
                    </td>
                    {!hasRole(Roles.Student) && <td>{r.requesterName}</td>}
                    <td>
                      <Link to={`/requests/${r.id}`} className="link">
                        View
                      </Link>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}

        {data && (
          <Pagination
            page={data.page}
            totalPages={data.totalPages}
            totalCount={data.totalCount}
            onPage={setPage}
          />
        )}
      </div>
    </div>
  );
}
