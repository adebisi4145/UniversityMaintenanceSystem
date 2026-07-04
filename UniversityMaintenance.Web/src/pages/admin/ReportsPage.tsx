import { useEffect, useState } from 'react';
import { errorMessage } from '../../api/client';
import { reportsApi } from '../../api/endpoints';
import { useToast } from '../../components/Toast';
import type { ReportSummary } from '../../types';

export function ReportsPage() {
  const toast = useToast();
  const [summary, setSummary] = useState<ReportSummary | null>(null);

  useEffect(() => {
    reportsApi.summary().then(setSummary).catch((err) => toast.error(errorMessage(err)));
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const exportCsv = () => {
    if (!summary) return;
    const rows: string[] = ['Metric,Value', `Total Requests,${summary.totalRequests}`, `Total Users,${summary.totalUsers}`];
    const section = (title: string, obj: Record<string, number>) => {
      rows.push('', title);
      Object.entries(obj).forEach(([k, v]) => rows.push(`${k},${v}`));
    };
    section('By Status', summary.requestsByStatus);
    section('By Category', summary.requestsByCategory);
    section('By Priority', summary.requestsByPriority);

    const blob = new Blob([rows.join('\n')], { type: 'text/csv' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = 'maintenance-report.csv';
    a.click();
    URL.revokeObjectURL(url);
  };

  if (!summary) return <div className="page">Loading…</div>;

  return (
    <div className="page">
      <header className="page-head">
        <h1>Reports</h1>
        <button className="btn btn-secondary" onClick={exportCsv}>
          ⬇ Export CSV
        </button>
      </header>

      <section className="stat-grid">
        <div className="stat-card accent-blue">
          <div className="stat-value">{summary.totalRequests}</div>
          <div className="stat-label">Total Requests</div>
        </div>
        <div className="stat-card accent-violet">
          <div className="stat-value">{summary.totalUsers}</div>
          <div className="stat-label">Total Users</div>
        </div>
      </section>

      <div className="report-cols">
        <BreakdownCard title="By Status" data={summary.requestsByStatus} />
        <BreakdownCard title="By Category" data={summary.requestsByCategory} />
        <BreakdownCard title="By Priority" data={summary.requestsByPriority} />
      </div>
    </div>
  );
}

function BreakdownCard({ title, data }: { title: string; data: Record<string, number> }) {
  const total = Object.values(data).reduce((a, b) => a + b, 0);
  return (
    <div className="card">
      <h2 className="card-title">{title}</h2>
      {Object.keys(data).length === 0 ? (
        <p className="muted">No data.</p>
      ) : (
        <div className="bar-list">
          {Object.entries(data).map(([label, count]) => (
            <div key={label} className="bar-row">
              <span className="bar-label">{label}</span>
              <div className="bar-track">
                <div
                  className="bar-fill"
                  style={{ width: `${total ? Math.round((count / total) * 100) : 0}%` }}
                />
              </div>
              <span className="bar-value">{count}</span>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
