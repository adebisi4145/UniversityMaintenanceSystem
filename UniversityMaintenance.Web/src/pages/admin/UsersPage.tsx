import { useEffect, useState } from 'react';
import { errorMessage } from '../../api/client';
import { usersApi } from '../../api/endpoints';
import { Pagination } from '../../components/Pagination';
import { useToast } from '../../components/Toast';
import { useAuth } from '../../auth/AuthContext';
import { Roles, type PagedResult, type UserDto } from '../../types';

export function UsersPage() {
  const toast = useToast();
  const { user: me } = useAuth();
  const [data, setData] = useState<PagedResult<UserDto> | null>(null);
  const [search, setSearch] = useState('');
  const [page, setPage] = useState(1);
  const [showForm, setShowForm] = useState(false);

  const [form, setForm] = useState({
    firstName: '',
    lastName: '',
    email: '',
    password: '',
    role: Roles.Officer as string,
  });

  const load = () => {
    usersApi
      .getPaged(search, page, 8)
      .then(setData)
      .catch((err) => toast.error(errorMessage(err)));
  };

  useEffect(load, [search, page]); // eslint-disable-line react-hooks/exhaustive-deps

  const createUser = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await usersApi.create(form);
      toast.success('User created');
      setForm({ firstName: '', lastName: '', email: '', password: '', role: Roles.Officer });
      setShowForm(false);
      load();
    } catch (err) {
      toast.error(errorMessage(err));
    }
  };

  const removeUser = async (id: string) => {
    if (!confirm('Delete this user?')) return;
    try {
      await usersApi.remove(id);
      toast.success('User deleted');
      load();
    } catch (err) {
      toast.error(errorMessage(err));
    }
  };

  return (
    <div className="page">
      <header className="page-head">
        <h1>Users</h1>
        <button className="btn btn-primary" onClick={() => setShowForm((s) => !s)}>
          {showForm ? 'Close' : '+ New User'}
        </button>
      </header>

      {showForm && (
        <form onSubmit={createUser} className="card form">
          <div className="grid-2">
            <label className="field">
              <span>First name</span>
              <input
                required
                value={form.firstName}
                onChange={(e) => setForm({ ...form, firstName: e.target.value })}
              />
            </label>
            <label className="field">
              <span>Last name</span>
              <input
                required
                value={form.lastName}
                onChange={(e) => setForm({ ...form, lastName: e.target.value })}
              />
            </label>
          </div>
          <div className="grid-2">
            <label className="field">
              <span>Email</span>
              <input
                type="email"
                required
                value={form.email}
                onChange={(e) => setForm({ ...form, email: e.target.value })}
              />
            </label>
            <label className="field">
              <span>Password</span>
              <input
                type="password"
                required
                minLength={6}
                value={form.password}
                onChange={(e) => setForm({ ...form, password: e.target.value })}
              />
            </label>
          </div>
          <label className="field">
            <span>Role</span>
            <select value={form.role} onChange={(e) => setForm({ ...form, role: e.target.value })}>
              <option value={Roles.Officer}>Officer</option>
              <option value={Roles.Admin}>Admin</option>
              <option value={Roles.Student}>Student</option>
            </select>
          </label>
          <div className="form-actions">
            <button className="btn btn-primary" type="submit">
              Create user
            </button>
          </div>
        </form>
      )}

      <div className="filters card">
        <input
          className="filter-search"
          placeholder="Search name or email…"
          value={search}
          onChange={(e) => {
            setPage(1);
            setSearch(e.target.value);
          }}
        />
      </div>

      <div className="card">
        <div className="table-wrap">
          <table className="table">
            <thead>
              <tr>
                <th>Name</th>
                <th>Email</th>
                <th>Role</th>
                <th>Joined</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              {data?.items.map((u) => (
                <tr key={u.id}>
                  <td className="strong">{u.fullName}</td>
                  <td>{u.email}</td>
                  <td>
                    <span className="badge badge-role">{u.role}</span>
                  </td>
                  <td>{new Date(u.createdAt).toLocaleDateString()}</td>
                  <td>
                    {u.id !== me?.id && (
                      <button className="link danger" onClick={() => removeUser(u.id)}>
                        Delete
                      </button>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
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
