import { useCallback, useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { errorMessage, imageUrl } from '../api/client';
import { assignmentsApi, requestsApi, usersApi } from '../api/endpoints';
import { useAuth } from '../auth/AuthContext';
import { PriorityBadge, StatusBadge } from '../components/StatusBadge';
import { useToast } from '../components/Toast';
import {
  Roles,
  STATUS_LABELS,
  RequestStatus,
  type ServiceRequestDto,
  type StatusUpdateDto,
  type UserDto,
} from '../types';

export function RequestDetailPage() {
  const { id = '' } = useParams();
  const { hasRole } = useAuth();
  const toast = useToast();
  const navigate = useNavigate();

  const [request, setRequest] = useState<ServiceRequestDto | null>(null);
  const [history, setHistory] = useState<StatusUpdateDto[]>([]);
  const [officers, setOfficers] = useState<UserDto[]>([]);
  const [loading, setLoading] = useState(true);

  const [newStatus, setNewStatus] = useState<string>('');
  const [comment, setComment] = useState('');
  const [officerId, setOfficerId] = useState('');
  const [notes, setNotes] = useState('');

  const load = useCallback(async () => {
    try {
      const [req, hist] = await Promise.all([
        requestsApi.getById(id),
        requestsApi.getHistory(id),
      ]);
      setRequest(req);
      setHistory(hist);
      setNewStatus(String(req.status));
    } catch (err) {
      toast.error(errorMessage(err));
    } finally {
      setLoading(false);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [id]);

  useEffect(() => {
    load();
    if (hasRole(Roles.Admin)) usersApi.getOfficers().then(setOfficers).catch(() => undefined);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [id]);

  const canManage = hasRole(Roles.Officer, Roles.Admin);

  const submitStatus = async () => {
    try {
      await requestsApi.updateStatus(id, Number(newStatus), comment || undefined);
      toast.success('Status updated');
      setComment('');
      load();
    } catch (err) {
      toast.error(errorMessage(err));
    }
  };

  const submitAssign = async () => {
    if (!officerId) return toast.error('Select an officer');
    try {
      await assignmentsApi.assign(id, officerId, notes || undefined);
      toast.success('Request assigned');
      setNotes('');
      setOfficerId('');
      load();
    } catch (err) {
      toast.error(errorMessage(err));
    }
  };

  if (loading) return <div className="page">Loading…</div>;
  if (!request) return <div className="page">Request not found.</div>;

  const img = imageUrl(request.evidenceImagePath);

  return (
    <div className="page">
      <button className="link back" onClick={() => navigate(-1)}>
        ← Back
      </button>

      <div className="detail-grid">
        <div className="detail-main">
          <div className="card">
            <div className="detail-head">
              <div>
                <h1>{request.title}</h1>
                <p className="muted">
                  {request.categoryName} · {request.location}
                </p>
              </div>
              <div className="badge-col">
                <StatusBadge status={request.status} />
                <PriorityBadge priority={request.priority} />
              </div>
            </div>

            <p className="detail-desc">{request.description}</p>

            <dl className="detail-meta">
              <div>
                <dt>Requester</dt>
                <dd>{request.requesterName}</dd>
              </div>
              <div>
                <dt>Assigned officer</dt>
                <dd>{request.assignedOfficerName ?? '—'}</dd>
              </div>
              <div>
                <dt>Created</dt>
                <dd>{new Date(request.createdAt).toLocaleString()}</dd>
              </div>
            </dl>

            {img && (
              <div className="evidence">
                <span className="muted small">Evidence</span>
                <a href={img} target="_blank" rel="noreferrer">
                  <img src={img} alt="Fault evidence" />
                </a>
              </div>
            )}
          </div>

          <div className="card">
            <h2 className="card-title">Activity timeline</h2>
            <ol className="timeline">
              {history.map((h) => (
                <li key={h.id}>
                  <span className="timeline-dot" />
                  <div>
                    <div className="timeline-head">
                      {h.oldStatus === h.newStatus ? (
                        <strong>{STATUS_LABELS[h.newStatus]}</strong>
                      ) : (
                        <strong>
                          {STATUS_LABELS[h.oldStatus]} → {STATUS_LABELS[h.newStatus]}
                        </strong>
                      )}
                      <span className="muted small">
                        {new Date(h.createdAt).toLocaleString()}
                      </span>
                    </div>
                    <div className="muted small">by {h.changedByName}</div>
                    {h.comment && <div className="timeline-comment">{h.comment}</div>}
                  </div>
                </li>
              ))}
            </ol>
          </div>
        </div>

        {canManage && (
          <aside className="detail-side">
            <div className="card">
              <h2 className="card-title">Update status</h2>
              <label className="field">
                <span>New status</span>
                <select value={newStatus} onChange={(e) => setNewStatus(e.target.value)}>
                  {Object.entries(STATUS_LABELS).map(([value, label]) => (
                    <option key={value} value={value}>
                      {label}
                    </option>
                  ))}
                </select>
              </label>
              <label className="field">
                <span>Comment</span>
                <textarea
                  rows={3}
                  value={comment}
                  onChange={(e) => setComment(e.target.value)}
                  placeholder="Optional note…"
                />
              </label>
              <button className="btn btn-primary btn-block" onClick={submitStatus}>
                Save status
              </button>
            </div>

            {hasRole(Roles.Admin) && (
              <div className="card">
                <h2 className="card-title">Assign officer</h2>
                <label className="field">
                  <span>Officer</span>
                  <select value={officerId} onChange={(e) => setOfficerId(e.target.value)}>
                    <option value="">Select…</option>
                    {officers.map((o) => (
                      <option key={o.id} value={o.id}>
                        {o.fullName}
                      </option>
                    ))}
                  </select>
                </label>
                <label className="field">
                  <span>Notes</span>
                  <textarea
                    rows={2}
                    value={notes}
                    onChange={(e) => setNotes(e.target.value)}
                    placeholder="Optional…"
                  />
                </label>
                <button
                  className="btn btn-secondary btn-block"
                  onClick={submitAssign}
                  disabled={request.status === RequestStatus.Completed}
                >
                  Assign request
                </button>
              </div>
            )}
          </aside>
        )}
      </div>
    </div>
  );
}
