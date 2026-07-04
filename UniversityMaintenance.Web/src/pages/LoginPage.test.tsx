import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { BrowserRouter } from 'react-router-dom';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import { LoginPage } from './LoginPage';
import { AuthProvider } from '../auth/AuthContext';
import { ToastProvider } from '../components/Toast';
import { authApi } from '../api/endpoints';

vi.mock('../api/endpoints', () => ({
  authApi: { login: vi.fn(), register: vi.fn() },
}));

function renderLogin() {
  return render(
    <BrowserRouter>
      <ToastProvider>
        <AuthProvider>
          <LoginPage />
        </AuthProvider>
      </ToastProvider>
    </BrowserRouter>,
  );
}

describe('LoginPage', () => {
  beforeEach(() => vi.clearAllMocks());

  it('shows a validation error and does not call the API when the email is invalid', async () => {
    renderLogin();
    await userEvent.type(screen.getByLabelText(/email/i), 'not-an-email');
    await userEvent.type(screen.getByLabelText(/password/i), 'secret');
    await userEvent.click(screen.getByRole('button', { name: /sign in/i }));

    expect(await screen.findByText(/valid email/i)).toBeInTheDocument();
    expect(authApi.login).not.toHaveBeenCalled();
  });

  it('calls the login API with valid credentials', async () => {
    (authApi.login as ReturnType<typeof vi.fn>).mockResolvedValue({
      token: 't',
      expiresAt: new Date().toISOString(),
      user: { id: '1', firstName: 'A', lastName: 'B', fullName: 'A B', email: 'a@x.edu', role: 'Student', createdAt: '' },
    });

    renderLogin();
    await userEvent.type(screen.getByLabelText(/email/i), 'a@x.edu');
    await userEvent.type(screen.getByLabelText(/password/i), 'secret1');
    await userEvent.click(screen.getByRole('button', { name: /sign in/i }));

    await waitFor(() => expect(authApi.login).toHaveBeenCalledWith('a@x.edu', 'secret1'));
  });
});
