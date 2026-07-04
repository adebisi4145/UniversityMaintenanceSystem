import { createContext, useCallback, useContext, useMemo, useState, type ReactNode } from 'react';
import { authApi } from '../api/endpoints';
import { tokenStore } from '../api/client';
import type { UserDto } from '../types';

interface AuthState {
  user: UserDto | null;
  isAuthenticated: boolean;
  login: (email: string, password: string) => Promise<UserDto>;
  register: (firstName: string, lastName: string, email: string, password: string) => Promise<UserDto>;
  logout: () => void;
  hasRole: (...roles: string[]) => boolean;
}

const USER_KEY = 'ums_user';

const AuthContext = createContext<AuthState | undefined>(undefined);

function loadStoredUser(): UserDto | null {
  const raw = localStorage.getItem(USER_KEY);
  if (!raw || !tokenStore.get()) return null;
  try {
    return JSON.parse(raw) as UserDto;
  } catch {
    return null;
  }
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<UserDto | null>(loadStoredUser);

  const persist = useCallback((token: string, u: UserDto) => {
    tokenStore.set(token);
    localStorage.setItem(USER_KEY, JSON.stringify(u));
    setUser(u);
  }, []);

  const login = useCallback(
    async (email: string, password: string) => {
      const res = await authApi.login(email, password);
      persist(res.token, res.user);
      return res.user;
    },
    [persist],
  );

  const register = useCallback(
    async (firstName: string, lastName: string, email: string, password: string) => {
      const res = await authApi.register(firstName, lastName, email, password);
      persist(res.token, res.user);
      return res.user;
    },
    [persist],
  );

  const logout = useCallback(() => {
    tokenStore.clear();
    localStorage.removeItem(USER_KEY);
    setUser(null);
  }, []);

  const hasRole = useCallback(
    (...roles: string[]) => (user ? roles.includes(user.role) : false),
    [user],
  );

  const value = useMemo<AuthState>(
    () => ({ user, isAuthenticated: !!user, login, register, logout, hasRole }),
    [user, login, register, logout, hasRole],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

// eslint-disable-next-line react-refresh/only-export-components
export function useAuth(): AuthState {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used within AuthProvider');
  return ctx;
}
