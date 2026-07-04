import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { Link, useNavigate } from 'react-router-dom';
import { z } from 'zod';
import { errorMessage } from '../api/client';
import { useAuth } from '../auth/AuthContext';
import { useToast } from '../components/Toast';

const schema = z.object({
  email: z.string().email('Enter a valid email'),
  password: z.string().min(1, 'Password is required'),
});
type FormValues = z.infer<typeof schema>;

export function LoginPage() {
  const { login } = useAuth();
  const toast = useToast();
  const navigate = useNavigate();
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<FormValues>({ resolver: zodResolver(schema) });

  const onSubmit = async (values: FormValues) => {
    try {
      await login(values.email, values.password);
      toast.success('Welcome back!');
      navigate('/dashboard');
    } catch (err) {
      toast.error(errorMessage(err, 'Login failed'));
    }
  };

  return (
    <div className="auth-page">
      <div className="auth-card">
        <div className="auth-brand">
          <span className="brand-mark">UM</span>
          <h1>University Maintenance</h1>
          <p className="muted">Sign in to submit and track service requests.</p>
        </div>

        <form onSubmit={handleSubmit(onSubmit)} className="form" noValidate>
          <label className="field">
            <span>Email</span>
            <input type="email" autoComplete="email" {...register('email')} />
            {errors.email && <small className="err">{errors.email.message}</small>}
          </label>

          <label className="field">
            <span>Password</span>
            <input type="password" autoComplete="current-password" {...register('password')} />
            {errors.password && <small className="err">{errors.password.message}</small>}
          </label>

          <button className="btn btn-primary btn-block" type="submit" disabled={isSubmitting}>
            {isSubmitting ? 'Signing in…' : 'Sign in'}
          </button>
        </form>

        <p className="auth-alt">
          No account? <Link to="/register">Create one</Link>
        </p>
      </div>
    </div>
  );
}
