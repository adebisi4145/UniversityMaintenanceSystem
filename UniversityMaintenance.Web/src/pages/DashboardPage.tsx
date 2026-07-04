import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { errorMessage } from '../api/client';
import { reportsApi, requestsApi } from '../api/endpoints';
import { useAuth } from '../auth/AuthContext';
import { useToast } from '../components/Toast';
import { StatusBadge } from '../components/StatusBadge';
import { Roles, STATUS_LABELS, type ReportSummary, type ServiceRequestDto } from '../types';

export function DashboardPage() {
  const { user, hasRole } = useAuth();
  const toast = useToast();
  const [summary, setSummary] = useState<ReportSummary | null>(null);
  const [recent, setRecent] = useState<ServiceRequestDto[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    (async () => {
      try {
        const recentPage = await requestsApi.getPaged({ page: 1, pageSize: 5 });
        setRecent(recentPage.items);
        if (hasRole(Roles.Admin)) setSummary(await reportsApi.summary());
      } catch (err) {
        toast.error(errorMessage(err));
      } finally {
        setLoading(false);
      }
    })();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  return (
    <div className="page">
      <header className="page-head">
        <div>
          <h1>Welcome, {user?.firstName} 👋</h1>
          <p className="muted">
            {hasRole(Roles.Student) && 'Submit new maintenance requests and track their progress.'}
            {hasRole(Roles.Officer) && 'Here are the jobs assigned to you.'}
            {hasRole(Roles.Admin) && 'Overview of maintenance activity across the university.'}
          </p>
        </div>
        {hasRole(Roles.Student) && (
          <Link to="/requests/new" className="btn btn-primary">
            + New Request
          </Link>
        )}
      </header>

      {hasRole(Roles.Admin) && summary && (
        <section className="stat-grid">
          <StatCard label="Total Requests" value={summary.totalRequests} accent="blue" />
          <StatCard label="Total Users" value={summary.totalUsers} accent="violet" />
          <StatCard
            label="Completed"
            value={summary.requestsByStatus['Completed'] ?? 0}
            accent="green"
          />
          <StatCard
            label="In Progress"
            value={summary.requestsByStatus['InProgress'] ?? 0}
            accent="amber"
          />
        </section>
      )}

      {hasRole(Roles.Admin) && summary && (
        <section className="card">
          <h2 className="card-title">Requests by status</h2>
          <div className="bar-list">
            {Object.entries(STATUS_LABELS).map(([key, label]) => {
              const count = summary.requestsByStatus[label.replace(' ', '')] ?? 0;
              const pct = summary.totalRequests
                ? Math.round((count / summary.totalRequests) * 100)
                : 0;
              return (
                <div key={key} className="bar-row">
                  <span className="bar-label">{label}</span>
                  <div className="bar-track">
                    <div className="bar-fill" style={{ width: `${pct}%` }} />
                  </div>
                  <span className="bar-value">{count}</span>
                </div>
              );
            })}
          </div>
        </section>
      )}

      <section className="card">
        <div className="card-head">
          <h2 className="card-title">Recent requests</h2>
          <Link to="/requests" className="link">
            View all →
          </Link>
        </div>
        {loading ? (
          <p className="muted">Loading…</p>
        ) : recent.length === 0 ? (
          <p className="muted">No requests yet.</p>
        ) : (
          <ul className="recent-list">
            {recent.map((r) => (
              <li key={r.id}>
                <Link to={`/requests/${r.id}`} className="recent-item">
                  <div>
                    <div className="recent-title">{r.title}</div>
                    <div className="muted small">
                      {r.categoryName} · {r.location}
                    </div>
                  </div>
                  <StatusBadge status={r.status} />
                </Link>
              </li>
            ))}
          </ul>
        )}
      </section>
    </div>
  );
}

function StatCard({ label, value, accent }: { label: string; value: number; accent: string }) {
  return (
    <div className={`stat-card accent-${accent}`}>
      <div className="stat-value">{value}</div>
      <div className="stat-label">{label}</div>
    </div>
  );
}
