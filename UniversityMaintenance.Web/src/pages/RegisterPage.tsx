import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { Link, useNavigate } from 'react-router-dom';
import { z } from 'zod';
import { errorMessage } from '../api/client';
import { useAuth } from '../auth/AuthContext';
import { useToast } from '../components/Toast';

const schema = z.object({
  firstName: z.string().min(1, 'First name is required'),
  lastName: z.string().min(1, 'Last name is required'),
  email: z.string().email('Enter a valid email'),
  password: z.string().min(6, 'Password must be at least 6 characters'),
});
type FormValues = z.infer<typeof schema>;

export function RegisterPage() {
  const { register: registerUser } = useAuth();
  const toast = useToast();
  const navigate = useNavigate();
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<FormValues>({ resolver: zodResolver(schema) });

  const onSubmit = async (values: FormValues) => {
    try {
      await registerUser(values.firstName, values.lastName, values.email, values.password);
      toast.success('Account created!');
      navigate('/dashboard');
    } catch (err) {
      toast.error(errorMessage(err, 'Registration failed'));
    }
  };

  return (
    <div className="auth-page">
      <div className="auth-card">
        <div className="auth-brand">
          <span className="brand-mark">UM</span>
          <h1>Create your account</h1>
          <p className="muted">Register as a student or staff member.</p>
        </div>

        <form onSubmit={handleSubmit(onSubmit)} className="form" noValidate>
          <div className="grid-2">
            <label className="field">
              <span>First name</span>
              <input {...register('firstName')} />
              {errors.firstName && <small className="err">{errors.firstName.message}</small>}
            </label>
            <label className="field">
              <span>Last name</span>
              <input {...register('lastName')} />
              {errors.lastName && <small className="err">{errors.lastName.message}</small>}
            </label>
          </div>

          <label className="field">
            <span>Email</span>
            <input type="email" autoComplete="email" {...register('email')} />
            {errors.email && <small className="err">{errors.email.message}</small>}
          </label>

          <label className="field">
            <span>Password</span>
            <input type="password" autoComplete="new-password" {...register('password')} />
            {errors.password && <small className="err">{errors.password.message}</small>}
          </label>

          <button className="btn btn-primary btn-block" type="submit" disabled={isSubmitting}>
            {isSubmitting ? 'Creating…' : 'Create account'}
          </button>
        </form>

        <p className="auth-alt">
          Already registered? <Link to="/login">Sign in</Link>
        </p>
      </div>
    </div>
  );
}
