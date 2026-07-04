import { Navigate, Route, Routes } from 'react-router-dom';
import { ProtectedRoute } from './auth/ProtectedRoute';
import { Layout } from './components/Layout';
import { LoginPage } from './pages/LoginPage';
import { RegisterPage } from './pages/RegisterPage';
import { DashboardPage } from './pages/DashboardPage';
import { RequestsListPage } from './pages/RequestsListPage';
import { SubmitRequestPage } from './pages/SubmitRequestPage';
import { RequestDetailPage } from './pages/RequestDetailPage';
import { UsersPage } from './pages/admin/UsersPage';
import { CategoriesPage } from './pages/admin/CategoriesPage';
import { ReportsPage } from './pages/admin/ReportsPage';
import { Roles } from './types';

export default function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route path="/register" element={<RegisterPage />} />

      <Route element={<ProtectedRoute />}>
        <Route element={<Layout />}>
          <Route path="/dashboard" element={<DashboardPage />} />
          <Route path="/requests" element={<RequestsListPage />} />
          <Route path="/requests/:id" element={<RequestDetailPage />} />

          <Route element={<ProtectedRoute roles={[Roles.Student]} />}>
            <Route path="/requests/new" element={<SubmitRequestPage />} />
          </Route>

          <Route element={<ProtectedRoute roles={[Roles.Admin]} />}>
            <Route path="/admin/users" element={<UsersPage />} />
            <Route path="/admin/categories" element={<CategoriesPage />} />
            <Route path="/admin/reports" element={<ReportsPage />} />
          </Route>
        </Route>
      </Route>

      <Route path="/" element={<Navigate to="/dashboard" replace />} />
      <Route path="*" element={<Navigate to="/dashboard" replace />} />
    </Routes>
  );
}
