import { NavLink, Outlet, useNavigate } from 'react-router-dom';
import { useAuth } from '../auth/AuthContext';
import { Roles } from '../types';

export function Layout() {
  const { user, logout, hasRole } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <div className="app-shell">
      <aside className="sidebar">
        <div className="brand">
          <span className="brand-mark">UM</span>
          <div>
            <div className="brand-title">UniMaintain</div>
            <div className="brand-sub">Service Desk</div>
          </div>
        </div>

        <nav className="nav">
          <NavLink to="/dashboard" className="nav-link">
            Dashboard
          </NavLink>

          <NavLink to="/requests" className="nav-link">
            {hasRole(Roles.Student) ? 'My Requests' : hasRole(Roles.Officer) ? 'Assigned Jobs' : 'All Requests'}
          </NavLink>

          {hasRole(Roles.Student) && (
            <NavLink to="/requests/new" className="nav-link">
              Submit Request
            </NavLink>
          )}

          {hasRole(Roles.Admin) && (
            <>
              <div className="nav-heading">Administration</div>
              <NavLink to="/admin/users" className="nav-link">
                Users
              </NavLink>
              <NavLink to="/admin/categories" className="nav-link">
                Categories
              </NavLink>
              <NavLink to="/admin/reports" className="nav-link">
                Reports
              </NavLink>
            </>
          )}
        </nav>

        <div className="sidebar-footer">
          <div className="user-chip">
            <div className="user-avatar">{user?.firstName?.[0] ?? '?'}</div>
            <div className="user-meta">
              <div className="user-name">{user?.fullName}</div>
              <div className="user-role">{user?.role}</div>
            </div>
          </div>
          <button className="btn btn-ghost btn-block" onClick={handleLogout}>
            Sign out
          </button>
        </div>
      </aside>

      <main className="content">
        <Outlet />
      </main>
    </div>
  );
}
