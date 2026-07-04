import { zodResolver } from '@hookform/resolvers/zod';
import { useEffect, useState } from 'react';
import { useForm } from 'react-hook-form';
import { useNavigate } from 'react-router-dom';
import { z } from 'zod';
import { errorMessage } from '../api/client';
import { categoriesApi, requestsApi } from '../api/endpoints';
import { useToast } from '../components/Toast';
import { Priority, type CategoryDto } from '../types';

const schema = z.object({
  title: z.string().min(1, 'Title is required').max(200),
  categoryId: z.string().min(1, 'Choose a category'),
  location: z.string().min(1, 'Location is required'),
  priority: z.string(),
  description: z.string().min(1, 'Description is required').max(2000),
});
type FormValues = z.infer<typeof schema>;

export function SubmitRequestPage() {
  const toast = useToast();
  const navigate = useNavigate();
  const [categories, setCategories] = useState<CategoryDto[]>([]);
  const [file, setFile] = useState<File | null>(null);

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: { priority: String(Priority.Medium) },
  });

  useEffect(() => {
    categoriesApi.getAll().then(setCategories).catch((err) => toast.error(errorMessage(err)));
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const onSubmit = async (values: FormValues) => {
    try {
      const form = new FormData();
      form.append('Title', values.title);
      form.append('Description', values.description);
      form.append('Location', values.location);
      form.append('Priority', values.priority);
      form.append('CategoryId', values.categoryId);
      if (file) form.append('evidence', file);

      const created = await requestsApi.create(form);
      toast.success('Request submitted!');
      navigate(`/requests/${created.id}`);
    } catch (err) {
      toast.error(errorMessage(err, 'Could not submit request'));
    }
  };

  return (
    <div className="page narrow">
      <header className="page-head">
        <h1>Submit a Service Request</h1>
      </header>

      <form onSubmit={handleSubmit(onSubmit)} className="card form" noValidate>
        <label className="field">
          <span>Title</span>
          <input placeholder="e.g. Broken socket in Room 12" {...register('title')} />
          {errors.title && <small className="err">{errors.title.message}</small>}
        </label>

        <div className="grid-2">
          <label className="field">
            <span>Category</span>
            <select {...register('categoryId')}>
              <option value="">Select…</option>
              {categories.map((c) => (
                <option key={c.id} value={c.id}>
                  {c.name}
                </option>
              ))}
            </select>
            {errors.categoryId && <small className="err">{errors.categoryId.message}</small>}
          </label>

          <label className="field">
            <span>Priority</span>
            <select {...register('priority')}>
              <option value={Priority.Low}>Low</option>
              <option value={Priority.Medium}>Medium</option>
              <option value={Priority.High}>High</option>
            </select>
          </label>
        </div>

        <label className="field">
          <span>Location</span>
          <input placeholder="e.g. Block A, Room 12" {...register('location')} />
          {errors.location && <small className="err">{errors.location.message}</small>}
        </label>

        <label className="field">
          <span>Description</span>
          <textarea rows={5} placeholder="Describe the fault…" {...register('description')} />
          {errors.description && <small className="err">{errors.description.message}</small>}
        </label>

        <label className="field">
          <span>Evidence photo (optional)</span>
          <input
            type="file"
            accept="image/*"
            onChange={(e) => setFile(e.target.files?.[0] ?? null)}
          />
          {file && <small className="muted">{file.name}</small>}
        </label>

        <div className="form-actions">
          <button type="button" className="btn btn-ghost" onClick={() => navigate(-1)}>
            Cancel
          </button>
          <button type="submit" className="btn btn-primary" disabled={isSubmitting}>
            {isSubmitting ? 'Submitting…' : 'Submit Request'}
          </button>
        </div>
      </form>
    </div>
  );
}
